using System.Diagnostics;

namespace ImageMeasurementApp
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageConverters
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
    {
        // Find the appropriate page
        switch (page)
        {
            case ApplicationPage.MainPage:
                return new MainPage(viewModel as ApplicationViewModel);

            default:
                Debugger.Break();
                return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="BasePage"/> the specific <see cref="ApplicationPage"/> that is for that type of page
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static ApplicationPage ToApplicationPage(this BasePage page)
    {
        // Find application page that matches the base page
        if (page is MainPage)
            return ApplicationPage.MainPage;

        // Alert developer of issue
        Debugger.Break();
        return default(ApplicationPage);
    }
}
}
