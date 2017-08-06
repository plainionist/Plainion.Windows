﻿
using NUnit.Framework;
using Plainion.Windows.Controls.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace Plainion.Windows.Tests.Controls.Text
{
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    class NotePadTests
    {
        [Test]
        public void OnLoaded_WhenReceived_DefaultFontIsArialWithSize13()
        {
            var notepad = new NotePad();
            notepad.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

            Assert.That(notepad.Document.FontFamily.FamilyNames.Values, Is.EquivalentTo(new[] { "Arial" }));
            Assert.That(notepad.Document.FontSize, Is.EqualTo(13d));
        }

        [Test]
        public void Document_Set_DocumentOfEditorGetsUpdated()
        {
            var notepad = new NotePad();
            notepad.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

            Assert.That(notepad.Document, Is.SameAs(notepad.myEditor.Document));

            notepad.Document = new FlowDocument();

            Assert.That(notepad.Document, Is.SameAs(notepad.myEditor.Document));
        }
    }
}
