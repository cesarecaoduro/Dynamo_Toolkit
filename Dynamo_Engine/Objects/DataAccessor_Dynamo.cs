﻿using Autodesk.DesignScript.Runtime;
using BH.Engine.Dynamo;
using BH.Engine.Dynamo.Objects;
using BH.UI.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Dynamo.Objects
{
    [IsVisibleInDynamoLibrary(false)]
    public class DataAccessor_Dynamo : DataAccessor
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public object[] Inputs { get; set; } = new object[] { };

        public object[] Outputs { get; set; } = new object[] { };


        /*************************************/
        /**** Input Getter Methods        ****/
        /*************************************/

        public override T GetDataItem<T>(int index)
        {
            object aObject = GetInputAt(index).IToBHoM();

            if (aObject == null)
                return default(T);

            Type aType = aObject.GetType();

            if (aType == typeof(T) || typeof(T).IsAssignableFrom(aType))
                return (T)aObject;

            if(typeof(T) == typeof(double))
            {
                if (aObject is string)
                {
                    double aValue;
                    if (double.TryParse((string)aObject, out aValue))
                        return (T)(object)aValue;
                }
                else if (aObject is sbyte || aObject is byte || aObject is short || aObject is ushort || aObject is int || aObject is uint || aObject is long || aObject is ulong || aObject is float || aObject is double || aObject is decimal)
                {
                    return (T)(object)System.Convert.ToDouble(aObject);
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (aObject is string)
                {
                    int aValue;
                    if (int.TryParse((string)aObject, out aValue))
                        return (T)(object)aValue;
                }
                else if (aObject is sbyte || aObject is byte || aObject is short || aObject is ushort || aObject is int || aObject is uint || aObject is long || aObject is ulong || aObject is float || aObject is double || aObject is decimal)
                {
                    return (T)(object)System.Convert.ToInt32(aObject);
                }
            }

            return default(T);
        }

        /*************************************/

        public override List<T> GetDataList<T>(int index)
        {
            object content = GetInputAt(index);
            if (content is ListWrapper)
                content = ((ListWrapper)content).Items;
            IEnumerable data = content as IEnumerable;

            return data.Cast<object>().Select(x => x.IToBHoM()).Cast<T>().ToList();
        }

        /*************************************/

        public override List<List<T>> GetDataTree<T>(int index)
        {
            object content = GetInputAt(index);
            if (content is TreeWrapper)
                content = ((TreeWrapper)content).Items;
            IEnumerable<IEnumerable> data = content as IEnumerable<IEnumerable>;

            return data.Select(y => y.Cast<object>().Select(x => x.IToBHoM()).Cast<T>().ToList()).ToList();
        }


        /*************************************/
        /**** Output Setter Methods       ****/
        /*************************************/

        public override bool SetDataItem<T>(int index, T data)
        {
            SetOutputAt(index, data.IToDesignScript());
            return true;
        }

        /*************************************/

        public override bool SetDataList<T>(int index, IEnumerable<T> data)
        {
            SetOutputAt(index, data.Select(x => x.IToDesignScript()).ToList());
            return true;
        }

        /*************************************/

        public override bool SetDataTree<T>(int index, IEnumerable<IEnumerable<T>> data)
        {
            SetOutputAt(index, data.Select(y => y.Select(x => x.IToDesignScript()).ToList()).ToList());
            return true;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public object GetInputAt(int index)
        {
            if (index >= 0 && index < Inputs.Length)
                return Inputs[index];
            else
                return null;
        }

        /***************************************************/

        public void SetOutputAt(int index, object data)
        {
            if (index < 0)
                return;

            if (Outputs.Length <= index)
            {
                object[] outputs = Outputs;
                Array.Resize(ref outputs, index + 1);
                Outputs = outputs;
            }

            Outputs[index] = data;
        }

        /***************************************************/
    }
}