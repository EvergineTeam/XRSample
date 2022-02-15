using System;
using Evergine.Common.Graphics;
using Evergine.Editor.Extension;
using Evergine.Editor.Extension.Attributes;
using XRSample.Features.Manipulator;

namespace XRSample
{
    [CustomPanelEditor(typeof(ManipulatorComponent))]
    public class MyCustomClassEditor : PanelEditor
    {
        private ManipulatorComponent manipulator;

        protected override void Loaded()
        {
            base.Loaded();

            this.manipulator = this.Instance as ManipulatorComponent;
        }

        public override void GenerateUI()
        {
            base.GenerateUI();

            var panel = this.propertyPanelContainer;

            var minLimit = panel.AddButton("Set Min Limit", "Set Angle Min Limit", this.manipulator.SetMinLimit);
            var maxLimit = panel.AddButton("Set Max Limit", "Set Angle Max Limit", this.manipulator.SetMaxLimit);
        }
    }
}
