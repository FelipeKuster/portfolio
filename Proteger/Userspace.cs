using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using Proteger.Managers;
using Proteger.ProtectiveObjects.Curves;
using Proteger.ProtectiveObjects;
using Proteger.Windows;

namespace Proteger.Workspace
{
    public class Userspace : IWorkspaceItem, INotifyPropertyChanged
    {
        //Herança> NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        //Herança> WorkspaceItem

        private ObservableCollection<IWorkspaceItem> m_children;
        public int ID { get; private set; }
        public int OwnerID { get; private set; }
        public int WorkspaceID { get; private set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; private set; }
        public ObservableCollection<IWorkspaceItem> Children { get { return this.m_children; } set { m_children = value; OnPropertyChanged("Children"); } }
        public IWorkspaceItem Parent { get; set; }
        public WorkspaceItemType ItemType { get { return WorkspaceItemType.Root; } }
        public string IconSource { get { return @"/Proteger;component/Resources/Icons/archive.png"; } }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public WindowCurveView UserCurveView = null;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Userspace(int id, int ownerId, string name, DateTime tc)
        {
            this.ID = id;
            this.WorkspaceID = id;
            this.OwnerID = id;
            this.Name = name;
            this.DateCreated = tc;
            this.Initialize();
            this.IsExpanded = true;
            this.IsSelected = true;
        }

        public void LoadChilds()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            List<IWorkspaceItem> loadingList = new List<IWorkspaceItem>();
            loadingList.AddRange(Core.GetWorkspaceManager().LoadFolders(this));
            loadingList.AddRange(Core.GetWorkspaceManager().LoadStudies(this));
            Children = new ObservableCollection<IWorkspaceItem>(loadingList);
        }

        public void CheckPlotWindow(WorkspaceStudy contextStudy)
        {
            InvokeCurveView(contextStudy);
            ShowPlotView();
        }

        private void InvokeCurveView(WorkspaceStudy contextStudy)
        {
            UserCurveView = new WindowCurveView(contextStudy);
        }

        public void ShowPlotView()
        {
            if (UserCurveView.IsActive)
            {
                UserCurveView.Focus();
                return;
            }
            UserCurveView.SetFocus();
        }

        public void LoadStudy(WorkspaceStudy existingStudy)
        {
            this.CheckPlotWindow(existingStudy);
        }
    }
}
