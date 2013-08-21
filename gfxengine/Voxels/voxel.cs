using System;
using System.Collections.Generic;
using WOCL.Shared.Utils;

namespace WOCL.Client.GFX.Voxels
{
    public class Voxel
    {
        /// <summary>
        /// Image, that contains this voxel
        /// </summary>
        public VoxelImage Image;

        /// <summary>
        /// Color. Used only for leafs
        /// </summary>
        public Clr3fl Color;
        /// <summary>
        /// Depth level of current voxel. 
        /// 0 - root
        /// Image.Depth - leaf
        /// </summary>
        public int Level;
        public Vector3D Center;
        public double RelSize;
        private bool filled = false;
        public double LightLevel;

        public static implicit operator bool(Voxel v)
        {
            return v != null;
        }
        public static bool operator !(Voxel v)
        {
            return v == null;
        }
        public static implicit operator Clr3fl(Voxel v)
        {
            if (!v || !v.IsLeaf() || !v.Filled()) return Clr3fl.NULL;
            return v.Color;
        }

        public bool Filled() { return filled; }
        public bool IsLeaf() { return Level == Image.Depth; }
        public bool IsSurface;

        public static bool IsVisible(Voxel v)
        {
            if (!v) return false;
            return /*v.IsLeaf() && */v.Filled();
        }
        public bool IsSideVisible(int x,int y, int z)
        {
            return !IsVisible(GetNeighbour(x, y, z));
        }
        /// <summary>
        /// Recalculates value of IsSurface and returns it value
        /// </summary>
        /// <returns></returns>
        public bool CheckSides()
        {
            IsSurface = false;
            if (IsSideVisible(-1, 0, 0))
            {
                IsSurface = true;
                return true;
            }
            if (IsSideVisible(1, 0, 0))
            {
                IsSurface = true;
                return true;
            }
            if (IsSideVisible(0, -1, 0))
            {
                IsSurface = true;
                return true;
            }
            if (IsSideVisible(0, 1, 0))
            {
                IsSurface = true;
                return true;
            }
            if (IsSideVisible(0, 0, -1))
            {
                IsSurface = true;
                return true;
            }
            if (IsSideVisible(0, 0, 1))
            {
                IsSurface = true;
                return true;
            }
            return false;
        }

        private Voxel[,,] neighbours;
        /// <summary>
        /// Unsafe cAll!!! use only [-1..1]!!!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Voxel GetNeighbour(int x, int y, int z)
        {
            if (Level == 0)
                return (z == 0 && y == 0 && z == 0) ? this : null;
            return neighbours[x + 1, y + 1, z + 1];
        }
        
        public Voxel[, ,] Childs;
        public Voxel Parent;

        public void SetColor(Clr3fl color)
        {
            if (!filled)
            {
                filled = true;
                Color = color;

                CheckSides();

                var nv = GetNeighbour(-1, 0, 0);
                if (nv) nv.CheckSides();

                nv = GetNeighbour(1, 0, 0);
                if (nv) nv.CheckSides();

                nv = GetNeighbour(0, -1, 0);
                if (nv) nv.CheckSides();

                nv = GetNeighbour(0, 1, 0);
                if (nv) nv.CheckSides();

                nv = GetNeighbour(0, 0, -1);
                if (nv) nv.CheckSides();

                nv = GetNeighbour(0, 0, 1);
                if (nv) nv.CheckSides();
            }
            else
            {
                Color = color;   
            }
        }
        public void Clear()
        {
            if (!filled) return;
            filled = false;
            IsSurface = false;

            var nv = GetNeighbour(-1, 0, 0);
            if (nv) nv.CheckSides();

            nv = GetNeighbour(1, 0, 0);
            if (nv) nv.CheckSides();

            nv = GetNeighbour(0, -1, 0);
            if (nv) nv.CheckSides();

            nv = GetNeighbour(0, 1, 0);
            if (nv) nv.CheckSides();

            nv = GetNeighbour(0, 0, -1);
            if (nv) nv.CheckSides();

            nv = GetNeighbour(0, 0, 1);
            if (nv) nv.CheckSides();
        }
#region RECURSIVE FUNCTIONS
        public delegate void DOperateVoxel(Voxel v);
        public static Voxel R_Init(VoxelImage img, int level, Vector3D center, Voxel parent)
        {
            var v = new Voxel { Center = center, Image = img, Level = level, RelSize = Math.Pow(2, -level) };
            if (level != 0)
                v.Parent = parent;
            if (!v.IsLeaf())
            {
                v.Childs = new Voxel[2, 2, 2];
                var cs = v.RelSize/4;
                var nl = level + 1;
                v.Childs[0, 0, 0] = R_Init(img, nl, center + new Vector3D(-cs, -cs, -cs), v);
                v.Childs[0, 0, 1] = R_Init(img, nl, center + new Vector3D(-cs, -cs, cs), v);
                v.Childs[0, 1, 0] = R_Init(img, nl, center + new Vector3D(-cs, cs, -cs), v);
                v.Childs[0, 1, 1] = R_Init(img, nl, center + new Vector3D(-cs, cs, cs), v);
                v.Childs[1, 0, 0] = R_Init(img, nl, center + new Vector3D(cs, -cs, -cs), v);
                v.Childs[1, 0, 1] = R_Init(img, nl, center + new Vector3D(cs, -cs, cs), v);
                v.Childs[1, 1, 0] = R_Init(img, nl, center + new Vector3D(cs, cs, -cs), v);
                v.Childs[1, 1, 1] = R_Init(img, nl, center + new Vector3D(cs, cs, cs), v);
            }
            return v;
        }
        public void R_FillUpNeighbours()
        {
            if (!IsLeaf())
            {
                Childs[0, 0, 0].R_FillUpNeighbours();
                Childs[0, 0, 1].R_FillUpNeighbours();
                Childs[0, 1, 0].R_FillUpNeighbours();
                Childs[0, 1, 1].R_FillUpNeighbours();
                Childs[1, 0, 0].R_FillUpNeighbours();
                Childs[1, 0, 1].R_FillUpNeighbours();
                Childs[1, 1, 0].R_FillUpNeighbours();
                Childs[1, 1, 1].R_FillUpNeighbours();
                return;
            }
            neighbours = new Voxel[3, 3, 3];
            neighbours[0, 0, 0] = Image[Center.x - RelSize, Center.y - RelSize, Center.z - RelSize];
            neighbours[0, 0, 1] = Image[Center.x - RelSize, Center.y - RelSize, Center.z];
            neighbours[0, 1, 0] = Image[Center.x - RelSize, Center.y, Center.z - RelSize];
            neighbours[0, 1, 1] = Image[Center.x - RelSize, Center.y, Center.z];
            neighbours[1, 0, 0] = Image[Center.x, Center.y - RelSize, Center.z - RelSize];
            neighbours[1, 0, 1] = Image[Center.x, Center.y - RelSize, Center.z];
            neighbours[1, 1, 0] = Image[Center.x, Center.y, Center.z - RelSize];
            neighbours[1, 1, 1] = this;
        }
        internal Voxel R_FindVoxel(double x, double y, double z)
        {
            if (IsLeaf()) return this;
            var X = (x <= Center.x ? 0 : 1);
            var Y = (y <= Center.y ? 0 : 1);
            var Z = (z <= Center.z ? 0 : 1);
            return Childs[X, Y, Z].R_FindVoxel(x, y, z);
        }
        public void R_GetSurface(List<Voxel> voxels)
        {
            if (IsLeaf())
            {
                if (IsSurface) voxels.Add(this);
                return;
            }
            Childs[0, 0, 0].R_GetSurface(voxels);
            Childs[0, 0, 1].R_GetSurface(voxels);
            Childs[0, 1, 0].R_GetSurface(voxels);
            Childs[0, 1, 1].R_GetSurface(voxels);
            Childs[1, 0, 0].R_GetSurface(voxels);
            Childs[1, 0, 1].R_GetSurface(voxels);
            Childs[1, 1, 0].R_GetSurface(voxels);
            Childs[1, 1, 1].R_GetSurface(voxels);
        }
        public void R_GetFilled(List<Voxel> voxels)
        {
            if (IsLeaf())
            {
                if (filled) voxels.Add(this);
                return;
            }
            Childs[0, 0, 0].R_GetFilled(voxels);
            Childs[0, 0, 1].R_GetFilled(voxels);
            Childs[0, 1, 0].R_GetFilled(voxels);
            Childs[0, 1, 1].R_GetFilled(voxels);
            Childs[1, 0, 0].R_GetFilled(voxels);
            Childs[1, 0, 1].R_GetFilled(voxels);
            Childs[1, 1, 0].R_GetFilled(voxels);
            Childs[1, 1, 1].R_GetFilled(voxels);
        }
        public void R_OperateAll(DOperateVoxel fOperateVoxel)
        {
            if (IsLeaf())
            {
                fOperateVoxel(this);
                return;
            }
            Childs[0, 0, 0].R_OperateAll(fOperateVoxel);
            Childs[0, 0, 1].R_OperateAll(fOperateVoxel);
            Childs[0, 1, 0].R_OperateAll(fOperateVoxel);
            Childs[0, 1, 1].R_OperateAll(fOperateVoxel);
            Childs[1, 0, 0].R_OperateAll(fOperateVoxel);
            Childs[1, 0, 1].R_OperateAll(fOperateVoxel);
            Childs[1, 1, 0].R_OperateAll(fOperateVoxel);
            Childs[1, 1, 1].R_OperateAll(fOperateVoxel);
        }
        public void R_OperateFilled(DOperateVoxel fOperateVoxel)
        {
            if (IsLeaf())
            {
                if (filled) fOperateVoxel(this);
                return;
            }
            Childs[0, 0, 0].R_OperateFilled(fOperateVoxel);
            Childs[0, 0, 1].R_OperateFilled(fOperateVoxel);
            Childs[0, 1, 0].R_OperateFilled(fOperateVoxel);
            Childs[0, 1, 1].R_OperateFilled(fOperateVoxel);
            Childs[1, 0, 0].R_OperateFilled(fOperateVoxel);
            Childs[1, 0, 1].R_OperateFilled(fOperateVoxel);
            Childs[1, 1, 0].R_OperateFilled(fOperateVoxel);
            Childs[1, 1, 1].R_OperateFilled(fOperateVoxel);
        }
        public void R_OperateNotFilled(DOperateVoxel fOperateVoxel)
        {
            if (IsLeaf())
            {
                if (!filled) fOperateVoxel(this);
                return;
            }
            Childs[0, 0, 0].R_OperateNotFilled(fOperateVoxel);
            Childs[0, 0, 1].R_OperateNotFilled(fOperateVoxel);
            Childs[0, 1, 0].R_OperateNotFilled(fOperateVoxel);
            Childs[0, 1, 1].R_OperateNotFilled(fOperateVoxel);
            Childs[1, 0, 0].R_OperateNotFilled(fOperateVoxel);
            Childs[1, 0, 1].R_OperateNotFilled(fOperateVoxel);
            Childs[1, 1, 0].R_OperateNotFilled(fOperateVoxel);
            Childs[1, 1, 1].R_OperateNotFilled(fOperateVoxel);
        }
        public void R_OperateSurface(DOperateVoxel fOperateVoxel)
        {
            if (IsLeaf())
            {
                if (IsSurface) fOperateVoxel(this);
                return;
            }
            Childs[0, 0, 0].R_OperateSurface(fOperateVoxel);
            Childs[0, 0, 1].R_OperateSurface(fOperateVoxel);
            Childs[0, 1, 0].R_OperateSurface(fOperateVoxel);
            Childs[0, 1, 1].R_OperateSurface(fOperateVoxel);
            Childs[1, 0, 0].R_OperateSurface(fOperateVoxel);
            Childs[1, 0, 1].R_OperateSurface(fOperateVoxel);
            Childs[1, 1, 0].R_OperateSurface(fOperateVoxel);
            Childs[1, 1, 1].R_OperateSurface(fOperateVoxel);
        }
#endregion
#region PAINTSTUFF
        public static void _FPaintTrigonometryShit0(Voxel v)
        {
            v.SetColor(new Clr3fl(v.Center));
        }
        public static void _FPaintTrigonometryShit1(Voxel v)
        {
            v.SetColor(new Clr3fl(Math.Cos(v.Center.x), Math.Sin(v.Center.y), v.Center.z));
        }
        public static void _FPaintBrighter(Voxel v)
        {
            ((Clr3fl)v).ModifyBrightness(2.0);
        }
        public static void _FPaintDarker(Voxel v)
        {
            ((Clr3fl)v).ModifyBrightness(.5);
        }
#endregion

#region INTERSETCTIONS
        public enum eCSC:int
        {
            UNDEF = 0,
            x0 = 1,
            x1 = 2,
            xx = 3,
            y0 = 4,
            y1 = 8,
            yy = 12,
            z0 = 16,
            z1 = 32,
            zz = 48
        }
        public int GetCSCode(Vector3D A)
        {
            var i = (int)eCSC.UNDEF;
            var hs = Math.Pow(2, Level)/2;
            if (A.x < Center.x - hs)
                i |= (int) eCSC.x0;
            else if (A.x > Center.x + hs)
                i |= (int) eCSC.x1;
            else
                i |= (int)eCSC.xx;
            if (A.y < Center.y - hs)
                i |= (int)eCSC.y0;
            else if (A.y > Center.y + hs)
                i |= (int)eCSC.y1;
            else
                i |= (int)eCSC.yy;
            if (A.z < Center.z - hs)
                i |= (int)eCSC.z0;
            else if (A.z > Center.z + hs)
                i |= (int)eCSC.z1;
            else
                i |= (int)eCSC.zz;
            return i;
        }
        public bool CSCEquals(int A, int B)
        {
            if (CSCHas(A, eCSC.x0) && CSCHas(B, eCSC.x0))
                return true;
            if (CSCHas(A, eCSC.x1) && CSCHas(B, eCSC.x1))
                return true;
            if (CSCHas(A, eCSC.y0) && CSCHas(B, eCSC.y0))
                return true;
            if (CSCHas(A, eCSC.y1) && CSCHas(B, eCSC.y1))
                return true;
            if (CSCHas(A, eCSC.z0) && CSCHas(B, eCSC.z0))
                return true;
            return CSCHas(A, eCSC.z1) && CSCHas(B, eCSC.z1);
        }

        public bool CSCHas(int A, eCSC f)
        {
            return ((A & (int) f) > 0);
        }
#endregion
    }
}
