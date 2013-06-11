using Gtk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daggerfall.Player
{
    public static class EntryPoint
    {
        static void Main(string[] args)
        {
            Application.Init("Daggerfall", ref args);
#if false
			var viewer = new Viewer() {
			};

			viewer.DeleteEvent += (o, a) => Application.Quit();
			viewer.Show();
			Application.Run();
#else
            RunGame();
#endif
        }

        static void RunGame()
        {
            string path = GetPath();
            if (path == null) return;
            new DaggerfallGame(path).Run();
            //new GtkApp().Run();
        }

        static string GetPath()
        {
            string path = Properties.Settings.Default.DaggerfallPath;
            int check;

            while ((check = CheckPath(ref path)) != 1)
            {
                if (check < 0)
                    return null;
                var chooser = new Gtk.FileChooserDialog("Select the path Daggerfall is in.", null, FileChooserAction.SelectFolder,
                    "Select", ResponseType.Accept, "Cancel", ResponseType.Cancel);

                chooser.Show();
                if (chooser.Run() == (int)ResponseType.Accept)
                {
                    path = chooser.Filename;
                }
                else
                    return null;
                chooser.Destroy();
            }

            Properties.Settings.Default.DaggerfallPath = path;
            Properties.Settings.Default.Save();
            return path;
        }

        static int CheckPath(ref string path)
        {
            if (path == null || path == "")
            {
                if (new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.OkCancel, "Please select the path you've installed Daggerfall to. You can select either the main folder or the ARENA2 folder.").Run() == (int)ResponseType.Cancel)
                    return -1;
                return 0;
            }
            else
            {
                if (!File.Exists(path + Path.DirectorySeparatorChar + "SKY00.DAT"))
                {
                    string subpath = path + Path.DirectorySeparatorChar + "ARENA2";
                    if (Directory.Exists(subpath))
                    {
                        path = subpath;
                        return CheckPath(ref path);
                    }
                }

                if (!File.Exists(path + Path.DirectorySeparatorChar + "ARCH3D.BSA"))
                {
                    if (new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.OkCancel, "Daggerfall seems to be copied to this path, but it has not been installed in DOS. Please do this or hit cancel and wait for me to figure out the compression.").Run() == (int)ResponseType.Cancel)
                        return -1;
                    return 0;
                }
            }
            return 1;
        }
    }
}
