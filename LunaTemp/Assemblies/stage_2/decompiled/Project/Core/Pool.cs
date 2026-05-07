using System.Collections.Generic;
using UnityEngine;

namespace Project.Core
{
	public sealed class Pool<T> where T : Component
	{
		private readonly T _prefab;

		private readonly Transform _parent;

		private readonly Stack<T> _free = new Stack<T>();

		public Pool(T prefab, Transform parent, int prewarm = 0)
		{
			_prefab = prefab;
			_parent = parent;
			for (int i = 0; i < prewarm; i++)
			{
				Release(CreateInstance());
			}
		}

		public T Rent()
		{
			T inst = ((_free.Count > 0) ? _free.Pop() : CreateInstance());
			inst.gameObject.SetActive(true);
			return inst;
		}

		public void Release(T instance)
		{
			if (!((Object)instance == (Object)null))
			{
				instance.gameObject.SetActive(false);
				instance.transform.SetParent(_parent, false);
				_free.Push(instance);
			}
		}

		private T CreateInstance()
		{
			T inst = Object.Instantiate(_prefab, _parent);
			inst.gameObject.SetActive(false);
			return inst;
		}
	}
}
