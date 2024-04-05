using System.Collections.Generic;

using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "level", menuName = "Scriptable Objects/Level", order = 1)]
	public class LevelScriptableObject : ScriptableObject
	{
		public Node Root;
		public List<Node> Nodes = new List<Node>();
		public Rect rect;
	}
}
