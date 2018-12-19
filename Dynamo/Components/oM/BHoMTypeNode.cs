﻿using Dynamo.Graph.Nodes;
using BH.UI.Dynamo.Templates;
using System;
using ProtoCore.AST.AssociativeAST;
using System.Collections.Generic;
using System.Xml;
using Dynamo.Graph;

namespace BH.UI.Dynamo.Components
{
    [NodeName("Type (old)")]
    [NodeDescription("Creates a type to choose from the context menu")]
    [NodeCategory("BHoM.oM")]
    [IsDesignScriptCompatible]
    public class BHoMTypeNode : NodeModel
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public Type Type { get; set; } = null;


        /*******************************************/
        /**** Constructors                      ****/
        /*******************************************/

        public BHoMTypeNode()
        {
            OutPortData.Add(new PortData("  ", "selected type"));
            RegisterAllPorts();
        }


        /*******************************************/
        /**** Override Methods                  ****/
        /*******************************************/

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Make sure the enum has been assigned
            if (Type == null)
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };


            // Create the Build assignment for outpout 0
            List<AssociativeNode> arguments = new List<AssociativeNode> { AstFactory.BuildStringNode(Type.AssemblyQualifiedName) };
            AssociativeNode functionCall = AstFactory.BuildFunctionCall("BH.UI.BHoM.Methods.Create", "BHoMType", arguments);
            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }

        /*******************************************/

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);
            var xmlDoc = element.OwnerDocument;

            if (Type != null)
            {
                var typeName = xmlDoc.CreateElement("TypeName");
                typeName.SetAttribute("value", Type.AssemblyQualifiedName);
                element.AppendChild(typeName);
            }
        }

        /*******************************************/

        protected override void DeserializeCore(XmlElement element, SaveContext context)
        {
            base.DeserializeCore(element, context);

            Type type = null;
            string methodName = "";
            List<Type> paramTypes = new List<Type>();

            foreach (XmlNode node in element.ChildNodes)
            {
                switch (node.Name)
                {
                    case "TypeName":
                        if (node.Attributes != null && node.Attributes["value"] != null && node.Attributes["value"].Value != null)
                        {
                            string typeString = node.Attributes["value"].Value;

                            //Fix for namespace change in structure
                            if (typeString.Contains("oM.Structural"))
                            {
                                typeString = typeString.Replace("oM.Structural", "oM.Structure");
                            }
                            Type = Type.GetType(typeString);
                        }
                        break;
                }
            }
        }

        /***************************************************/
    }
}