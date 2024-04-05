using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game
{
	[Serializable]
	public class Node
	{
		public int Id;
		public Vector2 Position;
		public int ParentCount;
		[SerializeReference] public List<Node> Childrens = new List<Node>();

		public Node Clone()
		{
			var node = new Node()
			{
				Id = Id,
				Position = Position,
				ParentCount = ParentCount,
				Childrens = new List<Node>()
			};
			foreach (var child in Childrens)
			{
				node.Childrens.Add(child.Clone());
			}
			return node;
		}

#if UNITY_EDITOR
		public Node Search(int id)
		{
			foreach (var child in Childrens)
			{
				if (child.Id == id)
				{
					return child;
				}
			}
			foreach (var child in Childrens)
			{
				var node = child.Search(id);
				if (node != null)
				{
					return node;
				}
			}
			return null;
		}

		public void Denoise()
		{
			var nodeNeedRemove = new List<Node>();
			foreach (var child1 in Childrens)
			{
				foreach (var child2 in Childrens)
				{
					if (child1 != child2)
					{
						if (child2.Search(child1.Id) != null)
						{
							nodeNeedRemove.Add(child1);
							break;
						}
					}
				}
			}
			foreach(var node in nodeNeedRemove)
			{
				--node.ParentCount;
				Childrens.Remove(node);
			}

			foreach (var child in Childrens)
			{
				child.Denoise();
			}
		}
#endif
	}
}
