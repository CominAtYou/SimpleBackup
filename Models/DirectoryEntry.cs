using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBackup.Helpers;

namespace SimpleBackup.Models;
public class DirectoryEntry
{
    public string Name
    {
        get;
    }

    public string ParentPath
    {
        get;
    }

    public DirectoryEntry(string TargetPath)
    {
        var info = new DirectoryInfo(TargetPath);
        Name = info.Name;
        ParentPath = info.Parent != null ? info.Parent.ToString() : "Main_DirectoryItem_SubtextNoParent".GetLocalized();
    }

    public override string ToString()
    {
        if (ParentPath == "Main_DirectoryItem_SubtextNoParent".GetLocalized())
        {
            return Name;
        }
        else return Path.Join(ParentPath, Name);
    }
}
