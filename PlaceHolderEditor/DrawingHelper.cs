﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceHolderEditor
{
    internal static class DrawingHelper
    {
        public static RectangleF MoveRectangleF(this RectangleF rect, PointF vector)
        {
            return new RectangleF(Add(rect.Location, vector), rect.Size);
        }

        public static PointF Add(this PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static PointF Subtract(this PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

		private static double Distance(PointF A, PointF B)
		{
			if (float.IsNaN(A.X) || float.IsNaN(B.X) || float.IsNaN(A.Y) || float.IsNaN(B.Y))
			{
				return double.MaxValue;
			}
			return Math.Sqrt(Math.Pow(A.X - B.X, 2.0) + Math.Pow(A.Y - B.Y, 2.0));
		}

        public static IGH_Attributes FindAttributeByGrip(this GH_Document doc, PointF pt, bool bLimitToOutside, bool bIncludeInputs, bool bIncludeOutputs, int SearchRadius = 12)
        {
            int index = -1;
            double num = double.MaxValue;
            for (int i = doc.AttributeCount - 1; i >= 0; i += -1)
            {
                IGH_Attributes iGH_Attributes = doc.Attributes[i];
                if (!(iGH_Attributes.DocObject is IGH_Param))
                {
                    continue;
                }
                if (bIncludeInputs && iGH_Attributes.HasInputGrip && (!bLimitToOutside || iGH_Attributes.InputGrip.X > pt.X))
                {
                    double num2 = Distance(iGH_Attributes.InputGrip, pt);
                    if (num2 < num)
                    {
                        num = num2;
                        index = i;
                    }
                }
                if (bIncludeOutputs && iGH_Attributes.HasOutputGrip && (!bLimitToOutside || iGH_Attributes.OutputGrip.X < pt.X))
                {
                    double num3 = Distance(iGH_Attributes.OutputGrip, pt);
                    if (num3 < num)
                    {
                        num = num3;
                        index = i;
                    }
                }
            }
            if (num > (double)SearchRadius)
            {
                return null;
            }
            return doc.Attributes[index];
        }
        public static GH_RelevantObjectData RelevantObjectAtPoint(this GH_Document doc, PointF pt, GH_RelevantObjectFilter searchFilter)
        {
            IList<IGH_Attributes> attributes = doc.Attributes;
            for (int i = attributes.Count - 1; i >= 0; i += -1)
            {
                IGH_Attributes iGH_Attributes = attributes[i];
                if (!(iGH_Attributes is GH_GroupAttributes))
                {
                    if (iGH_Attributes.HasInputGrip && (searchFilter & GH_RelevantObjectFilter.InputGrips) == GH_RelevantObjectFilter.InputGrips && Distance(pt, iGH_Attributes.InputGrip) < 12.0)
                    {
                        GH_RelevantObjectData gH_RelevantObjectData = new GH_RelevantObjectData(pt);
                        gH_RelevantObjectData.CreateGripData(iGH_Attributes.DocObject, is_input: true);
                        return gH_RelevantObjectData;
                    }
                    if (iGH_Attributes.HasOutputGrip && (searchFilter & GH_RelevantObjectFilter.OutputGrips) == GH_RelevantObjectFilter.OutputGrips && Distance(pt, iGH_Attributes.OutputGrip) < 12.0)
                    {
                        GH_RelevantObjectData gH_RelevantObjectData2 = new GH_RelevantObjectData(pt);
                        gH_RelevantObjectData2.CreateGripData(iGH_Attributes.DocObject, is_input: false);
                        return gH_RelevantObjectData2;
                    }
                    if ((searchFilter & GH_RelevantObjectFilter.Attributes) == GH_RelevantObjectFilter.Attributes && iGH_Attributes.IsPickRegion(pt))
                    {
                        GH_RelevantObjectData gH_RelevantObjectData3 = new GH_RelevantObjectData(pt);
                        gH_RelevantObjectData3.CreateObjectData(iGH_Attributes.DocObject);
                        return gH_RelevantObjectData3;
                    }
                }
            }
            _ = searchFilter & GH_RelevantObjectFilter.Wires;
            _ = 1024;
            if ((searchFilter & GH_RelevantObjectFilter.Groups) == GH_RelevantObjectFilter.Groups)
            {
                for (int j = doc.Objects.Count - 1; j >= 0; j += -1)
                {
                    if (doc.Objects[j] is GH_Group && doc.Objects[j].Attributes.IsPickRegion(pt))
                    {
                        GH_RelevantObjectData gH_RelevantObjectData4 = new GH_RelevantObjectData(pt);
                        gH_RelevantObjectData4.CreateGroupData((GH_Group)doc.Objects[j]);
                        return gH_RelevantObjectData4;
                    }
                }
            }
            return null;
        }
    }
}
