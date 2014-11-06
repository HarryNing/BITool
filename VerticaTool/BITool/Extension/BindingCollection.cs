using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITool.Extension
{
    public class BindingCollection<T> : BindingList<T>
    {
        private bool isSorted;
        private PropertyDescriptor sortProperty;
        private ListSortDirection sortDirection;

        public BindingCollection()
        {
        }

        public BindingCollection(IList<T> list)
            : base(list)
        {
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            var items = Items as List<T>;

            if (items != null)
            {
                var pc = new ObjectPropertyCompare<T>(property, direction);
                try
                {
                    items.Sort(pc);
                    isSorted = true;
                }
                catch (Exception)
                {
                    isSorted = false;
                }
            }
            else
            {
                isSorted = false;
            }

            sortProperty = property;
            sortDirection = direction;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
