﻿using System;
using System.ComponentModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions.EventMonitoring;

namespace FluentAssertions.Silverlight.Specs
{
    [TestClass]
    public class EventMonitoringSpecs
    {
        [TestMethod]
        public void When_a_property_changed_event_for_a_specific_property_was_not_raised_it_should_throw()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //----------------------------------------------------------------------------------------------------------
            var subject = new EventRaisingClass();
            subject.MonitorEvents();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.ShouldRaisePropertyChangeFor(x => x.SomeProperty, "the property was changed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.ShouldThrow<AssertFailedException>().WithMessage(
                "Expected object <" + subject + "> to raise event \"PropertyChanged\" because the property was changed, but it did not.");
        }
        
        [TestMethod]
        public void When_a_property_changed_event_for_a_specific_property_was_raised_but_without_sender_it_should_throw()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //----------------------------------------------------------------------------------------------------------
            var subject = new EventRaisingClass();
            subject.MonitorEvents();
            subject.RaiseEventWithoutSender();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject
                .ShouldRaisePropertyChangeFor(x => x.SomeProperty, "the property was changed")
                .WithSender(subject);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.ShouldThrow<AssertFailedException>().WithMessage("Expected sender <" + subject + ">, but found <null>.");
        }

        [TestMethod]
        public void When_a_property_changed_event_was_raised_for_the_right_property_it_should_not_throw()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //----------------------------------------------------------------------------------------------------------
            var subject = new EventRaisingClass();
            subject.MonitorEvents();
            subject.RaiseEventWithSenderAndPropertyName("SomeProperty");
            subject.RaiseEventWithSenderAndPropertyName("SomeOtherProperty");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.ShouldRaisePropertyChangeFor(x => x.SomeProperty);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.ShouldNotThrow();
        }

        internal class EventRaisingClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public void RaiseEventWithoutSender()
            {
                PropertyChanged(null, new PropertyChangedEventArgs("SomeProperty"));
            }

            public void RaiseEventWithSender()
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SomeProperty"));
            }

            public void RaiseEventWithSenderAndPropertyName(string propertyName)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            public string SomeProperty { get; set; }
        }
    }
}