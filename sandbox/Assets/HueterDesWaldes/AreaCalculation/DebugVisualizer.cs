using System.Collections.Generic;
using UnityEngine;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    public class DebugVisualizer : MonoBehaviour
    {
        //parameters for child instances
        public Color color = Color.cyan;
        public List<Vector3> tiles = new List<Vector3>();
        private float tileLengthX;
        private float tileLengthZ;
        public List<Vector3> verticalLines = new List<Vector3>();

        public static DebugVisualizer GetInstance(Transform parent, string instanceName)
        {
            Transform dvTransform = parent.Find(instanceName);
            GameObject dvGameObject;
            if (dvTransform == null)
            {
                dvGameObject = new GameObject();
                dvGameObject.transform.parent = parent;
                dvGameObject.name = instanceName;
                return dvGameObject.AddComponent<DebugVisualizer>();
            }
            else
            {
                dvGameObject = dvTransform.gameObject;
                return dvGameObject.GetComponent<DebugVisualizer>();
            }
        }

        /// <summary>
        /// Use to draw positions as tiles on screen.
        /// </summary>
        /// <param name="tiles">list of positions</param>
        public void VisualDebuggingTiles(List<Vector3> tiles, float tileLengthX, float tileLengthZ)
        {
            this.tiles = tiles;
            this.tileLengthX = tileLengthX;
            this.tileLengthZ = tileLengthZ;
        }

        /// <summary>
        /// Use to draw positions as vertical lines on screen.
        /// </summary>
        /// <param name="lines">list of positions</param>
        public void VisualDebuggingLines(List<Vector3> lines)
        {
            this.verticalLines = lines;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (Vector3 from in verticalLines)
                DrawVerticalLine(from);

            DrawTiles();
        }

        private void DrawVerticalLine(Vector3 from)
        {
            Vector3 to = from;
            to.y = 0;
            Debug.DrawLine(from, to, color);
        }

        private void DrawTiles()
        {
            if (tiles == null)
                return;

            //calculate tile size in meters from resolution
            float marginX = tileLengthX * 0.1f;
            float marginZ = tileLengthZ * 0.1f;

            Vector3 size = new Vector3(tileLengthX - marginX, 0.1f, tileLengthZ - marginZ);
            foreach (Vector3 pos in tiles)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
}
