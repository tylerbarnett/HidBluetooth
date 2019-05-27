// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace HidBluetooth
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TestCommandBtn { get; set; }

        [Action ("TestCmd_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TestCmd_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (TestCommandBtn != null) {
                TestCommandBtn.Dispose ();
                TestCommandBtn = null;
            }
        }
    }
}