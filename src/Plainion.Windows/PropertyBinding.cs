using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Plainion.Windows
{
    public static class PropertyBinding
    {
        private class BindingEntry
        {
            public string Id;
            public INotifyPropertyChanged Source;
            public string PropertyName;
            public EventHandler<PropertyChangedEventArgs> Handler;
        }

        // weakly tracks all the anonymous event handlers
        // see: http://stackoverflow.com/questions/15225547/weakeventmanager-holds-reference-to-subscriber
        private static ConditionalWeakTable<INotifyPropertyChanged, List<BindingEntry>> myBindings = new ConditionalWeakTable<INotifyPropertyChanged, List<BindingEntry>>();

        /// <summary>
        /// Binds two properties where both declaring types implement INotifyPropertyChanged with BindingMode.TwoWay.
        /// </summary>
        /// <returns>
        /// The id of the binding which can be used to unbind source and target.
        /// </returns>
        public static BindingId Bind<T>(Expression<Func<T>> source, Expression<Func<T>> target)
        {
            return Bind(source, target, BindingMode.TwoWay);
        }

        /// <summary>
        /// Binds two properties where both declaring types implement INotifyPropertyChanged.
        /// Supported BindingModes: OneWay, OneWayToSource, TwoWay
        /// </summary>
        /// <returns>
        /// The id of the binding which can be used to unbind source and target.
        /// </returns>
        public static BindingId Bind<T>(Expression<Func<T>> source, Expression<Func<T>> target, BindingMode mode)
        {
            return Bind(BindableProperty.Create(source), BindableProperty.Create(target), mode);
        }

        /// <summary>
        /// Binds two properties where both declaring types implement INotifyPropertyChanged.
        /// Supported BindingModes: OneWay, OneWayToSource, TwoWay
        /// </summary>
        /// <returns>
        /// The id of the binding which can be used to unbind source and target.
        /// </returns>
        public static BindingId Bind(BindableProperty source, BindableProperty target, BindingMode mode)
        {
            Contract.Requires(mode == BindingMode.OneWay || mode == BindingMode.OneWayToSource || mode == BindingMode.TwoWay,
                "BindingMode not supported: " + mode);

            var id = new BindingId();

            if (mode == BindingMode.TwoWay || mode == BindingMode.OneWay)
            {
                id.Source = source.Owner;
                id.SourceBindingId = BindHandler(source, (s, e) => target.SetValue(source.GetValue()));
            }

            if (mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource)
            {
                id.Target = target.Owner;
                id.TargetBindingId = BindHandler(target, (s, e) => source.SetValue(target.GetValue()));
            }

            return id;
        }

        private static string BindHandler(BindableProperty prop, EventHandler<PropertyChangedEventArgs> handler)
        {
            List<BindingEntry> entries;
            if (!myBindings.TryGetValue(prop.Owner, out entries))
            {
                entries = new List<BindingEntry>();
                myBindings.Add(prop.Owner, entries);
            }

            var id = Guid.NewGuid().ToString();
            entries.Add(new BindingEntry
            {
                Id = id,
                Source = prop.Owner,
                PropertyName = prop.PropertyName,
                Handler = handler
            });

            PropertyChangedEventManager.AddHandler(prop.Owner, handler, prop.PropertyName);

            return id;
        }

        /// <summary>
        /// Observes a property changes notified with INotifyPropertyChanged.
        /// </summary>
        /// <returns>
        /// The id of the binding which can be used to unbind source and target.
        /// </returns>
        public static BindingId Observe<T>(Expression<Func<T>> source, EventHandler<PropertyChangedEventArgs> handler)
        {
            var prop = BindableProperty.Create(source);
            var id = BindHandler(prop, handler);

            return new BindingId
            {
                Source = prop.Owner,
                SourceBindingId = id
            };
        }

        /// <summary>
        /// Removes the binding specified by the given id.
        /// </summary>
        public static void Unbind(BindingId id)
        {
            TryUnbind(id.Source, id.SourceBindingId);
            TryUnbind(id.Target, id.TargetBindingId);
        }

        private static void TryUnbind(INotifyPropertyChanged owner, string id)
        {
            if (owner == null || id == null)
            {
                return;
            }

            List<BindingEntry> entries;
            if (!myBindings.TryGetValue(owner, out entries))
            {
                return;
            }

            var entry = entries.Single(e => e.Id == id);
            PropertyChangedEventManager.RemoveHandler(entry.Source, entry.Handler, entry.PropertyName);
            entries.Remove(entry);
        }
    }

    public class BindingId
    {
        internal INotifyPropertyChanged Source { get; set; }
        internal string SourceBindingId { get; set; }
        internal INotifyPropertyChanged Target { get; set; }
        internal string TargetBindingId { get; set; }
    }
}
