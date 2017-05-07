﻿using System;
using System.Windows.Controls;
using System.Windows.Input;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    partial class DocumentNavigationPane : UserControl
    {
        public DocumentNavigationPane()
        {
            Root = new NavigationNode();

            CreateChildCommand = new DelegateCommand<NavigationNode>(OnCreateChild);
            DeleteCommand = new DelegateCommand<NavigationNode>(n => ((NavigationNode)n.Parent).Children.Remove(n));

            var dragDropBehavior = new DragDropBehavior(Root);
            DropCommand = new DelegateCommand<NodeDropRequest>(dragDropBehavior.ApplyDrop);

            InitializeComponent();
        }

        private void OnCreateChild(NavigationNode parent)
        {
            var node = new NavigationNode
            {
                Parent = parent,
                Name = "<new>",
            };
            parent.Children.Add(node);

            node.IsSelected = true;
            parent.IsExpanded = true;
        }

        public NavigationNode Root { get; private set; }
        
        public ICommand CreateChildCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DropCommand { get; private set; }
    }
}