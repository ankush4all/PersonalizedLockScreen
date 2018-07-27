using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.System.UserProfile;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading;

namespace App1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        CloudQueueMessage previousMessage;
        CancellationTokenSource tokenSource;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.CheckSignal();
        }

        private async void CheckSignal()
        {
            LockScreenStorage storage = new LockScreenStorage();
            tokenSource = new CancellationTokenSource();

            while (true)
            {
                if (previousMessage != null)
                {
                    tokenSource.Cancel();
                    storage.DeleteMessage(previousMessage);
                    //Wait for 5 seconds

                    await Task.Delay(5 * 1000);
                }

                tokenSource = new CancellationTokenSource();

                // #HACK_FOR_DEMO
                // Hacking to set the default screen to something
                var folder = ApplicationData.Current.LocalFolder;
                var currentPath = folder.Path;
                var imagePath = currentPath + @"\destImages\Default\default.jpg";
                await this.SetBackground(imagePath);
                await Task.Delay(5 * 1000);
                // #END_DEMO

                previousMessage = await storage.ReadFromQueue();
                Task lockScreenTask = Task.Factory.StartNew(() => this.SetLockScreenImage(previousMessage), tokenSource.Token);

                //Wait for 1 minute before attempting to read next message
                await Task.Delay(1 * 60 * 1000);
            }
        }


        /// <summary>
        /// Unit function to set the background
        /// </summary>
        /// <param name="sitefile"></param>
        /// <returns></returns>
        private async Task<bool> SetBackground(string sitefile)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(sitefile);
            return await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(storageFile);
        }

        /// <summary>
        /// Sets the lockscreen image
        /// </summary>
        private async void SetLockScreenImage(CloudQueueMessage message)
        {
            if (UserProfilePersonalizationSettings.IsSupported() && message != null)
            {
                var folder = ApplicationData.Current.LocalFolder;
                var currentPath = folder.Path;
                var imagePath = currentPath + @"\destImages\";

                // #HACK_FOR_DEMO
                // hacking this to Hyderabad for demo so that people can relate
                // string targetDest = message.AsString;
                string targetDest = "Hyderabad";                
                imagePath += targetDest + @"\";

                // #HACK_FOR_DEMO
                // uncomment it later
                this.CopyImages(targetDest, @"C:\Users\ankja\Pictures\Hack-2018\", imagePath);

                string[] siteFiles = Directory.GetFiles(imagePath);
                while (true)
                {
                    foreach (var sitefile in siteFiles)
                    {
                        await this.SetBackground(sitefile);

                        // terminate when the cancellation is requested
                        if (tokenSource.IsCancellationRequested)
                        {                          
                            return;
                        }

                        // rotate the image every 10 seconds
                        await Task.Delay(10 * 1000);
                    }
                }
            }
            else
            {
                // do nothing
            }
        }

        private void CopyImages(string targetDest, string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                // do nothing
            }
            else
            {
                Directory.CreateDirectory(destinationPath);
            }

            var sourceFolderPath = sourcePath + @"/" + targetDest + @"/";

            var sourceFiles = Directory.GetFiles(sourceFolderPath);
            foreach (var filePath in sourceFiles)
            {
                string fileName = Path.GetFileName(filePath);
                var destFile = destinationPath + fileName;
                if (File.Exists(destFile))
                {
                    // do nothing
                }
                else
                {
                    File.Copy(filePath, destFile);
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //Frame rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            //if (rootFrame == null)
            //{
            //    // Create a Frame to act as the navigation context and navigate to the first page
            //    rootFrame = new Frame();

            //    rootFrame.NavigationFailed += OnNavigationFailed;

            //    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //    {
            //        //TODO: Load state from previously suspended application
            //    }

            //    // Place the frame in the current Window
            //    Window.Current.Content = rootFrame;
            //}

            //if (e.PrelaunchActivated == false)
            //{
            //    if (rootFrame.Content == null)
            //    {
            //        // When the navigation stack isn't restored navigate to the first page,
            //        // configuring the new page by passing required information as a navigation
            //        // parameter
            //        rootFrame.Navigate(typeof(MainPage), e.Arguments);
            //    }
            //    // Ensure the current window is active
            //    Window.Current.Activate();
            //}
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
