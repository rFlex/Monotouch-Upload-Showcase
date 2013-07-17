using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace TestUpload
{
	public partial class TestUploadViewController : UIViewController
	{

		////////////////////////////////////
		// VARIABLES
		////////////////////

		private UIImagePickerController pickerController;
		private byte[] toUploadData;
		private Uploader uploader;

		////////////////////////////////////
		// CONSTRUCTORS
		////////////////////

		public TestUploadViewController (IntPtr handle) : base (handle) {
		}

		////////////////////////////////////
		// METHODS
		////////////////////

		public override void ViewDidLoad () {
			base.ViewDidLoad ();

			this.pickerController = new UIImagePickerController ();
			this.pickerController.Canceled += this.HandleCanceledPickingMedia;
			this.pickerController.FinishedPickingMedia += this.HandleFinishedPickingMedia;

			this.pickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;

			this.startUploadingButton.TouchUpInside += this.HandleUploadButtonPressed;
			this.chooseImageButton.TouchUpInside += this.HandleChooseImageButtonPressed;

			this.imageUploadView.Layer.ShadowColor = UIColor.Black.CGColor;
			this.imageUploadView.Layer.ShadowOffset = new SizeF (2, 2);

			this.imageUploadView.Layer.CornerRadius = 6;

			this.SetImageForUpload (null);
		}

		public void SetImageForUpload(UIImage image) {
			if (image == null) {
				this.imageUploadView.Hidden = true;
			} else {
				this.toUploadData = ExportToJpg(image);

				this.presentationImageView.Image = image;
				this.ConfigureAsStartUploading ();

				this.imageUploadView.Hidden = false;
			}
		}

		private void HandleUploadEnded(string response, Exception thrownException) {
			string message = null;

			if (thrownException == null) {
				message = "The upload succeed: " + response;
			} else {
				message = "The upload failed: " + thrownException.Message;
				Console.WriteLine (thrownException.StackTrace);

			}
			UIAlertView alertView = new UIAlertView ("Upload finished", message, null, "OK");
			alertView.Show ();
			this.uploader = null;
			this.ConfigureAsStartUploading ();
		}

		private void ConfigureAsStartUploading() {
			this.startUploadingButton.SetTitle ("Start uploading", UIControlState.Normal);
			this.informationsLabel.Text = "Tap below to start uploading";
			this.SetProgressRatio (0);
		}

		private void ConfigureAsStopUploading() {
			this.startUploadingButton.SetTitle ("Cancel upload", UIControlState.Normal);
			this.informationsLabel.Text = "Uploading started.";
		}

		public static byte[] ExportToJpg(UIImage image) {
			using (NSData data = image.AsJPEG(1)) {
				byte[] binaryData = new byte[data.Length];

				// Copy the unmanaged data hold by the NSData to a new managed byte array.
				System.Runtime.InteropServices.Marshal.Copy(data.Bytes, binaryData, 0, (int)data.Length);

				return binaryData;
			}
		}

		private void HandleProgressChanged(Uploader uploader) {
			this.BeginInvokeOnMainThread (delegate {
				this.SetProgressRatio(uploader.UploadedRatio);
			});
		}

		private void StartUpload() {
			this.uploader = new Uploader ("http://ever-development.elasticbeanstalk.com/api/1/admin/upload_test");
			this.uploader.OnProgressChanged += this.HandleProgressChanged;

			this.uploader.DataToUpload = this.toUploadData;
			this.uploader.UploadAsync (delegate(string response, Exception thrownException) {
				this.BeginInvokeOnMainThread(delegate {
					this.HandleUploadEnded(response, thrownException);
				});
			});
			this.ConfigureAsStopUploading();
		}

		private void StopUpload() {
			if (this.uploader != null) {
				this.uploader.Dispose ();
			}
		}

		private void HandleUploadButtonPressed(object sender, EventArgs e) {
			if (this.uploader != null) {
				this.StopUpload ();
			} else {
				this.StartUpload ();
			}
		}

		private void HandleChooseImageButtonPressed(object sender, EventArgs e) {
			this.PresentViewController (this.pickerController, true, null);
		}

		private void HandleCanceledPickingMedia(object sender, EventArgs e) {
			this.pickerController.DismissViewController (true, null);
		}

		private void HandleFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e) {
			this.SetImageForUpload (e.OriginalImage);
			this.pickerController.DismissViewController (true, null);
		}

		public void SetProgressRatio(float progressRatio) {
			this.progressView.ProgressRatio = progressRatio;

			RectangleF overlayFrame = this.presentationImageView.Frame;
			overlayFrame.Height = this.presentationImageView.Frame.Height * (1 - progressRatio);

			this.imageOverlayView.Frame = overlayFrame;
		}

		protected override void Dispose (bool disposing) {
			base.Dispose (disposing);

			if (this.pickerController != null) {
				this.pickerController.Dispose ();
				this.pickerController = null;
			}
			if (this.uploader != null) {
				this.uploader.Dispose ();
				this.uploader = null;
			}
			this.ReleaseDesignerOutlets ();
		}

		////////////////////////////////////
		// GETTERS/SETTERS
		////////////////////



	}
}

