using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace IfcXplorer.ViewModels
{ 
    public class TreeViewBuilder
    {
        public static List<IXbimViewModel> ContainmentView(IModel model)
        {
            var svList = new List<IXbimViewModel>();

            var project = model.Instances.FirstOrDefault<IIfcProject>();
            if (project == null) return svList;

            svList.Add(new XbimModelViewModel(project, null));
            foreach (var child in svList)
            {
                LazyLoadAll(child);
            }
            return svList;
        }

        private static void LazyLoadAll(IXbimViewModel parent)
        {

            foreach (var child in parent.Children)
            {
                LazyLoadAll(child);
            }

        }
    }
}