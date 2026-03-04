// See https://aka.ms/new-console-template for more information

using Gtk;

var application = Gtk.Application.New("com.zcdevelopment.shelly", Gio.ApplicationFlags.DefaultFlags);

application.OnActivate += (sender, args) =>
{
    var mainBuilder = Builder.NewFromFile("UiFiles/MainWindow.ui");
    var window = (ApplicationWindow)mainBuilder.GetObject("MainWindow")!;
    window.Application = application;

    var menuBuilder = Builder.NewFromFile("UiFiles/MainMenu.ui");
    var appMenu = (Gio.Menu)menuBuilder.GetObject("AppMenu")!;
    application.Menubar = appMenu;

    var quitAction = Gio.SimpleAction.New("quit", null);
    quitAction.OnActivate += (sender, args) => application.Quit();
    application.AddAction(quitAction);

    var preferencesAction = Gio.SimpleAction.New("preferences", null);
    preferencesAction.OnActivate += (sender, args) => Console.WriteLine("Preferences clicked");
    application.AddAction(preferencesAction);

    var aboutAction = Gio.SimpleAction.New("about", null);
    aboutAction.OnActivate += (sender, args) => Console.WriteLine("About clicked");
    application.AddAction(aboutAction);

    var contentArea = (Box)mainBuilder.GetObject("ContentArea")!;

    var homeBuilder = Builder.NewFromFile("UiFiles/HomeWindow.ui");
    var homeBox = (Box)homeBuilder.GetObject("HomeWindow")!;

    contentArea.Append(homeBox);

    window.Show();
};

return application.Run(args);