using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc.ViewModels;
using Xbim.Ifc4.Interfaces;

namespace IfcXplorer.ViewModels
{
    public class SpatialViewModel : CustomXbimViewModel
    {
        private readonly IIfcObjectDefinition _spatialStructure;
        private List<IXbimViewModel> _children;

        public override int EntityLabel => _spatialStructure.EntityLabel;
        public override IPersistEntity Entity => _spatialStructure;


        public override string Name => !string.IsNullOrWhiteSpace(_spatialStructure.Name)
            ? _spatialStructure.ExpressType.ExpressName.Substring(3) + " - " + _spatialStructure.Name.ToString()
            : _spatialStructure.ExpressType.ExpressName.Substring(3);

        public SpatialViewModel(IIfcSpatialStructureElement spatialStructure, IXbimViewModel parent)
        {
            if (spatialStructure == null) throw new ArgumentNullException(nameof(spatialStructure));
            Model = spatialStructure.Model;
            _spatialStructure = spatialStructure;
            CreatingParent = parent;
        }

        public SpatialViewModel(IIfcProject project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            Model = project.Model;
            _spatialStructure = project;
        }

        public override IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = new List<IXbimViewModel>();
                // list related items of type IfcSpatialStructureElement
                foreach (var aggregate in _spatialStructure.IsDecomposedBy)
                {
                    foreach (var subSpace in aggregate.RelatedObjects.OfType<IIfcSpatialStructureElement>())
                        _children.Add(new SpatialViewModel(subSpace, this));
                }

                // now add any contained elements
                // this will not happen in case item is IfcProject
                if (!(_spatialStructure is IIfcSpatialStructureElement spatialElem)) return _children;

                //Select all the disting type names of elements for this
                foreach (var type in spatialElem.ContainsElements.SelectMany(container => container.RelatedElements).Select(r => r.GetType()).Distinct())
                {
                    _children.Add(new ContainedElementsViewModel(spatialElem, type, this));
                }
                return _children;
            }
        }
        
        

    }
}
