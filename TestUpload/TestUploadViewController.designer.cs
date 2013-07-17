// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace TestUpload
{
	[Register ("TestUploadViewController")]
	partial class TestUploadViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton chooseImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView imageOverlayView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView imageUploadView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel informationsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView presentationImageView { get; set; }

		[Outlet]
		TestUpload.ProgressView progressView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton startUploadingButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (chooseImageButton != null) {
				chooseImageButton.Dispose ();
				chooseImageButton = null;
			}

			if (imageUploadView != null) {
				imageUploadView.Dispose ();
				imageUploadView = null;
			}

			if (informationsLabel != null) {
				informationsLabel.Dispose ();
				informationsLabel = null;
			}

			if (presentationImageView != null) {
				presentationImageView.Dispose ();
				presentationImageView = null;
			}

			if (progressView != null) {
				progressView.Dispose ();
				progressView = null;
			}

			if (startUploadingButton != null) {
				startUploadingButton.Dispose ();
				startUploadingButton = null;
			}

			if (imageOverlayView != null) {
				imageOverlayView.Dispose ();
				imageOverlayView = null;
			}
		}
	}
}
