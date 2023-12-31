﻿using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MSDHSystem.iOS.Effects;

[assembly: ResolutionGroupName("MSDHSystem")]
[assembly: ExportEffect(typeof(RemoveEntryBordersEffect), nameof(RemoveEntryBordersEffect))]
namespace MSDHSystem.iOS.Effects
{
    public class RemoveEntryBordersEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var textField = this.Control as UITextField;

            if (textField is null)
                throw new NotImplementedException();

            textField.BorderStyle = UITextBorderStyle.None;
            textField.BackgroundColor = Color.Transparent.ToUIColor();
        }

        protected override void OnDetached()
        {
        }
    }
}