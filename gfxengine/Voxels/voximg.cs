using System;
using WOCL.Shared.Utils;

namespace WOCL.Client.GFX.Voxels
{
    public class VoxelImage
    {
        /// <summary>
        /// Scale for image
        /// 1.0 means whole image (1x1x1) in OGL coords
        /// </summary>
        public double Scale;
        public Vector3D Rotation;
        /// <summary>
        /// Leaf depth level. Dont make to big, make it [0..9]
        /// 0 - root is a leaf
        /// </summary>
        public int Depth;
        public long VoxelsCount
        {
            get
            {
                return (long)Math.Pow(8, Depth);
            }
        }

        public Voxel Root;
        public void Init(int maxdepth)
        {
            Root = Voxel.R_Init(this, 0, Vector3D.NULL, null);
            Root.R_FillUpNeighbours();
        }

        public Voxel this[Vector3D RelCoord]
        {
            get
            {
                return FindVoxel(RelCoord.x, RelCoord.y, RelCoord.z);
            }
        }
        public Voxel this[double x, double y, double z]
        {
            get
            {
                return FindVoxel(x, y, z);
            }
        }

        private Voxel FindVoxel(double x, double y, double z)
        {
            if (x >= -0.5 && x <= 0.5 && y >= -0.5 && y <= 0.5 && z >= -0.5 && z <= 0.5)
            {
                Root.R_FindVoxel(x, y, z);
            }
            return null;
        }
    }
}
