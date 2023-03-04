using AdapterDll;
using System.IO;
using System;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Linq;

namespace Source;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        LoadDlls();
    }

    public void LoadDlls()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "ext");

        if (!Directory.Exists(path))
            return;

        var dlls = Directory.EnumerateFiles(path, "*.dll");


        foreach (var dll in dlls)
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);

                var types = asm.GetTypes();

                var module = types.FirstOrDefault(type => typeof(IFunctionModule).IsAssignableFrom(type));

                if (module == null) 
                    return;

                var moduleObject = Activator.CreateInstance(module) as IFunctionModule;
                
                var stringEditor = moduleObject?.GetStringEditor();

                var button = new Button
                {
                    Content = stringEditor?.ButtonText,
                    Height = 50,
                    Width = 100,
                    Margin = new Thickness(5)
                };

                button.Click += (s, e) =>
                {
                    var text = inputTextBox.Text;
                    var result = stringEditor?.Function.Invoke(text);
                    outputTextBox.Text = result;
                };

                ButtonsPanel.Children.Add(button);
            }
            catch
            {
                /* Ignore non-clr dlls */
            }

        }

    }

}
