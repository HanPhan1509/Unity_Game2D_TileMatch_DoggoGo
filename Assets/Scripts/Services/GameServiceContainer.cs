using System.Collections.Generic;
using System;
namespace Services
{
	public class GameServiceContainer : IServiceProvider
	{
		private Dictionary<System.Type, object> Container = new Dictionary<System.Type, object>();
		/// <summary>
		/// Add service to container.
		/// </summary>
		/// <param name="interfaceType">type of service</param>
		/// <param name="service">service</param>
		public void AddService(System.Type interfaceType, object service)
		{
			if (interfaceType == null || service == null)
			{
				Logger.Error("Type null or service null.");
				return;
			}
			if (Container.ContainsKey(interfaceType))
			{
				Logger.Error("Object with type " + interfaceType.ToString() + " existed in Container");
				return;
			}
			Container.Add(interfaceType, service);
		}
		/// <summary>
		/// Add service to container that service is class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="provider"></param>
		public void AddService<T>(T provider) where T : class
		{
			if (provider == null)
			{
				Logger.Error("Service null.");
				return;
			}
			AddService(typeof(T), provider);
		}
		/// <summary>
		/// Get service from container.
		/// </summary>
		/// <param name="interfaceType">type of service</param>
		/// <returns></returns>
		public object GetService(System.Type interfaceType)
		{
			if (interfaceType == null)
			{
				Logger.Error("Type null.");
				return null;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				Logger.Error("Type " + interfaceType.ToString() + " doesn't exist in Container.");
				return null;
			}
			return Container[interfaceType];
		}
		/// <summary>
		/// Get service from container that service is class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				Logger.Error("Type null.");
				return null;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				Logger.Error("Type " + typeof(T).ToString() + " doesn't exist in Container.");
				return null;
			}
			return (T)Container[typeof(T)];
		}
		/// <summary>
		/// Remove service from container.
		/// </summary>
		/// <param name="interfaceType">type of service</param>
		public void RemoveService(System.Type interfaceType)
		{
			if (interfaceType == null)
			{
				Logger.Error("Type null.");
				return;
			}
			if (!Container.ContainsKey(interfaceType))
			{
				Logger.Error("Type " + interfaceType.ToString() + " doesn't exist in Container.");
				return;
			}
			Container.Remove(interfaceType);
		}
		/// <summary>
		/// Remove service from container that service is class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void RemoveService<T>() where T : class
		{
			if (typeof(T) == null)
			{
				Logger.Error("Type null.");
				return;
			}
			if (!Container.ContainsKey(typeof(T)))
			{
				Logger.Error("Type " + typeof(T).ToString() + " doesn't exist in Container.");
				return;
			}
			Container.Remove(typeof(T));
		}
	}
}
