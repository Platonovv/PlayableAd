using UnityEngine;

namespace Project.Gameplay.Targeting
{
    /// <summary>
    /// Бегущая пунктирная линия от игрока к цели + опциональный glow под целью.
    /// </summary>
    public sealed class TargetIndicator : MonoBehaviour
    {
        [SerializeField] private LineRenderer _line;
        [SerializeField] private GameObject _selectionGlow;
        [SerializeField] private float _heightOffset = 0.05f;
        [SerializeField] private float _dashWorldLength = 0.5f;
        [SerializeField] private float _flowSpeed = 1.5f;
        [SerializeField] private Color _color = new(1f, 0.85f, 0.2f, 1f);

        private Transform _from;
        private Transform _to;
        private Material _runtimeMaterial;
        private float _flowOffset;

        private void Awake()
        {
            if (_line == null) return;
            _line.enabled = false;
            if (_selectionGlow != null) _selectionGlow.SetActive(false);
            ConfigureLine();
        }

        private void ConfigureLine()
        {
            // Своя инстанс-материала — чтобы тайлинг текстуры обновлять покадрово, не трогая asset.
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent");
            _runtimeMaterial = new Material(shader)
            {
                name = "DashedIndicator",
                mainTexture = BuildDashTexture(),
                color = _color
            };
            _line.material = _runtimeMaterial;
            _line.textureMode = LineTextureMode.Tile;
            _line.startColor = _color;
            _line.endColor = _color;
            _line.useWorldSpace = true;
            if (_line.positionCount < 2) _line.positionCount = 2;
        }

        private Texture2D BuildDashTexture()
        {
            // 32×4: первая половина — opaque, вторая — alpha 0. Получается ровный пунктир.
            var tex = new Texture2D(32, 4, TextureFormat.RGBA32, false)
            {
                name = "DashTex",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };
            var px = new Color32[32 * 4];
            for (var y = 0; y < 4; y++)
            for (var x = 0; x < 32; x++)
            {
                var inDash = x < 16;
                px[y * 32 + x] = new Color32(255, 255, 255, (byte)(inDash ? 255 : 0));
            }
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return tex;
        }

        public void Show(Transform from, Transform to) => Show(from, to, _color);

        public void Show(Transform from, Transform to, Color color)
        {
            _from = from;
            _to = to;
            _line.enabled = true;
            SetColor(color);
            if (_selectionGlow != null)
            {
                _selectionGlow.SetActive(true);
                _selectionGlow.transform.SetParent(to, false);
                _selectionGlow.transform.localPosition = Vector3.zero;
            }
        }

        public void SetColor(Color color)
        {
            if (_runtimeMaterial != null) _runtimeMaterial.color = color;
            if (_line != null) { _line.startColor = color; _line.endColor = color; }
        }

        public void Hide()
        {
            _from = _to = null;
            _line.enabled = false;
            if (_selectionGlow != null)
            {
                _selectionGlow.transform.SetParent(transform, true);
                _selectionGlow.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (_from == null || _to == null) return;
            var a = _from.position; a.y += _heightOffset;
            var b = _to.position;   b.y += _heightOffset;
            _line.SetPosition(0, a);
            _line.SetPosition(1, b);

            // Пунктир тайлится по длине, и медленно «течёт» в сторону цели — даёт ощущение направления.
            var distance = Vector3.Distance(a, b);
            var tiles = Mathf.Max(1f, distance / Mathf.Max(0.0001f, _dashWorldLength));
            _flowOffset -= Time.deltaTime * _flowSpeed;
            if (_runtimeMaterial != null)
            {
                _runtimeMaterial.mainTextureScale  = new Vector2(tiles, 1f);
                _runtimeMaterial.mainTextureOffset = new Vector2(_flowOffset, 0f);
            }
        }

        private void OnDestroy()
        {
            if (_runtimeMaterial != null) Destroy(_runtimeMaterial);
        }
    }
}
