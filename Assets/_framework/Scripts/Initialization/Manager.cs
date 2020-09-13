using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
	public abstract class Manager<T> : MonoBehaviour, IInitializable
		where T : MonoBehaviour
	{
		public static bool HasInstance => _instance != null;

		#region Singleton

		private static object _lockObject = new object();
		private static bool _isExiting;
		
		private static T _instance;
		public static T Instance
		{
			get
			{
				lock (_lockObject)
				{
					return _instance;
				}
			}

			private set
			{
				if (_instance != null || _isExiting)
				{
					return;
				}

				_instance = value;
				var instanceTransform = _instance.transform;
				var instanceGameObject = _instance.gameObject;
				instanceTransform.SetParent(null);
				instanceTransform.position = Vector3.zero;
				instanceTransform.rotation = Quaternion.identity;
				instanceTransform.localScale = Vector3.one;
				instanceGameObject.name = typeof(T).Name;
				DontDestroyOnLoad(instanceGameObject);
			}
		}

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				Instance = this as T;
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			_isExiting = true;
		}

		#endregion Singleton

		#region IInitializable

		/// <summary>
		/// Invokes the action when the manager is initialized.
		/// </summary>
		/// <param name="eventHandler"></param>
		public static void InitializedAction(Action eventHandler)
		{
			if (HasInstance && Instance is IInitializable initializable)
			{
				if (initializable.IsInitialized)
				{
					eventHandler();
					return;
				}
			}

			OnInitializedEvent -= (eventHandler);
			OnInitializedEvent += (eventHandler);
		}

		public static event Action OnInitializedEvent;

		public virtual bool IsInitialized { get; protected set; }
		public virtual bool IsInitializing { get; protected set; }

		public virtual void Initialize()
		{
			// Early out if we are already initializing.
			if (IsInitialized || IsInitializing) return;

			// Early out if our singleton object is being destroyed.
			if (null == this || null == gameObject) return;

			StartCoroutine(InitializationRoutine());
		}

		protected virtual IEnumerator InitializationRoutine()
		{
			IsInitializing = true;
			yield return StartCoroutine(WaitForDependencies());
			yield return StartCoroutine(InitializeManager());
			IsInitializing = false;
			IsInitialized = true;
			OnInitializedEvent?.Invoke();
			
			yield break;
		}

		#endregion IInitializable

		protected virtual IEnumerator WaitForDependencies()
		{
			yield break;
		}

		protected virtual IEnumerator InitializeManager()
		{
			yield break;
		}
	}
}
