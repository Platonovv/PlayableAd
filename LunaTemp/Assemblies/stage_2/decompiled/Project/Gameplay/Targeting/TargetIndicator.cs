using UnityEngine;

namespace Project.Gameplay.Targeting
{
	public sealed class TargetIndicator : MonoBehaviour
	{
		[SerializeField]
		private LineRenderer _line;

		[SerializeField]
		private GameObject _selectionGlow;

		[SerializeField]
		private float _heightOffset = 0.05f;

		[SerializeField]
		private float _dashWorldLength = 0.5f;

		[SerializeField]
		private float _flowSpeed = 1.5f;

		[SerializeField]
		private Color _color = new Color(1f, 0.85f, 0.2f, 1f);

		private Transform _from;

		private Transform _to;

		private Material _runtimeMaterial;

		private float _flowOffset;

		private void Awake()
		{
			if (!(_line == null))
			{
				_line.enabled = false;
				if (_selectionGlow != null)
				{
					_selectionGlow.SetActive(false);
				}
				ConfigureLine();
			}
		}

		private void ConfigureLine()
		{
			Material baseMat = _line.sharedMaterial;
			if (baseMat != null)
			{
				_runtimeMaterial = new Material(baseMat)
				{
					name = "DashedIndicator"
				};
			}
			else
			{
				Shader shader = Shader.Find("Sprites/Default");
				if (shader == null)
				{
					shader = Shader.Find("Mobile/Diffuse");
				}
				if (shader == null)
				{
					_line.enabled = false;
					return;
				}
				_runtimeMaterial = new Material(shader)
				{
					name = "DashedIndicator"
				};
			}
			_runtimeMaterial.mainTexture = BuildDashTexture();
			_runtimeMaterial.color = _color;
			_line.material = _runtimeMaterial;
			_line.textureMode = LineTextureMode.Tile;
			_line.startColor = _color;
			_line.endColor = _color;
			_line.useWorldSpace = true;
			if (_line.positionCount < 2)
			{
				_line.positionCount = 2;
			}
		}

		private Texture2D BuildDashTexture()
		{
			Texture2D tex = new Texture2D(32, 4, TextureFormat.RGBA32, false)
			{
				name = "DashTex",
				wrapMode = TextureWrapMode.Repeat,
				filterMode = FilterMode.Bilinear
			};
			Color32[] px = new Color32[128];
			for (int y = 0; y < 4; y++)
			{
				for (int x = 0; x < 32; x++)
				{
					bool inDash = x < 16;
					px[y * 32 + x] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(inDash ? 255u : 0u));
				}
			}
			tex.SetPixels32(px);
			tex.Apply(false, true);
			return tex;
		}

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

		public void Hide()
		{
			_from = (_to = null);
			_line.enabled = false;
			if (_selectionGlow != null)
			{
				_selectionGlow.transform.SetParent(base.transform, true);
				_selectionGlow.SetActive(false);
			}
		}

		private void SetColor(Color color)
		{
			if (_runtimeMaterial != null)
			{
				_runtimeMaterial.color = color;
			}
			if (_line != null)
			{
				_line.startColor = color;
				_line.endColor = color;
			}
		}

		private void LateUpdate()
		{
			if (!(_from == null) && !(_to == null))
			{
				Vector3 a = _from.position;
				a.y += _heightOffset;
				Vector3 b = _to.position;
				b.y += _heightOffset;
				_line.SetPosition(0, a);
				_line.SetPosition(1, b);
				float distance = Vector3.Distance(a, b);
				float tiles = Mathf.Max(1f, distance / Mathf.Max(0.0001f, _dashWorldLength));
				_flowOffset -= Time.deltaTime * _flowSpeed;
				if (_runtimeMaterial != null)
				{
					_runtimeMaterial.mainTextureScale = new Vector2(tiles, 1f);
					_runtimeMaterial.mainTextureOffset = new Vector2(_flowOffset, 0f);
				}
			}
		}

		private void OnDestroy()
		{
			if (_runtimeMaterial != null)
			{
				Object.Destroy(_runtimeMaterial);
			}
		}
	}
}
