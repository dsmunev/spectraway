﻿using System;
using System.Threading.Tasks;
using NLog;
using SpectraWay.Localization;

namespace SpectraWay.Device.Stepper.L3Motor
{
    public class L3MotorStepper : IStepper
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Stepper _stepper;
        private double _currentDistance;
        private int _currentStep;

        public void Start()
        {

            try
            {
                _stepper?.Dispose();
                _stepper = new Stepper(16, 12, 11, 10, 9);
                _stepper.setSpeed(1000);
                GoToBase();
                IsStepperReady = true;
                CurrentDistance = MIN_DISTANCE;
            }
            catch (Exception e)
            {
                IsStepperReady = false;
                Logger.Error(e);
                
                //throw;
            }
            OnStatusChanged(IsStepperReady);
        }

        private void GoToBase()
        {
            _stepper.step(-5);
            _stepper.step(1);
        }

        private const double UNIT = 0.125;
        private const double MIN_DISTANCE = 1.25;

        public void Stop()
        {
            _stepper?.Dispose();
            IsStepperReady = false;
            OnStatusChanged(IsStepperReady);
        }

        public void InnerGoToDistance(double distance)
        {
            if(!IsStepperReady) Start();
            if(!IsStepperReady) return;
            if (distance < MIN_DISTANCE && CurrentDistance == MIN_DISTANCE)
            {
                //CurrentDistance = MIN_DISTANCE;
                return;
            }

            var realDistance = MIN_DISTANCE + Math.Round((distance - MIN_DISTANCE) / UNIT) * UNIT;
            var isBase = false;
            if (distance < MIN_DISTANCE)
            {
                realDistance = MIN_DISTANCE;
                isBase = true;
                Logger.Warn(StringResourceProvider.Instance[StringResourceProvider.Keys.RequestedDistanceLessThanMinDistance]);
                Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.CurrentDistanceSetAsPossibleMinimalDistance_PLACE_Mm].Value, MIN_DISTANCE);
            }

            var previousDistance = CurrentDistance;
            try
            {
                var delta = realDistance - previousDistance;
                var steps = (int)Math.Round(delta / UNIT);
                _stepper.step(steps);
                _currentStep = steps;
                CurrentDistance = realDistance;
                if (isBase)
                {
                    GoToBase();
                }

            }
            catch (Exception e)
            {
                LastError = e.Message + "\n" + e.InnerException?.Message + "\n" + e.StackTrace;
                Logger.Error(e);
            }
        }

        public void GoToDistance(double distance)
        {
            InnerGoToDistance(distance);
        }

        public async Task GoToDistanceAsync(double distance)
        {
            await Task.Run(() => InnerGoToDistance(distance));
        }

        public string LastError { get; private set; }
        public bool IsStepperReady { get; private set; }

        public double CurrentDistance
        {
            get { return _currentDistance; }
            private set
            {
                _currentDistance = value;
                OnDistanceChanged();
            }
        }

        public event StepperStatusChangedEventHandler StatusChanged;
        private void OnStatusChanged(bool isStepperReady)
        {
            var e = new StepperStatusChangedEventHandlerArgs(isStepperReady);
            StatusChanged?.Invoke(this, e);
        }

        private void OnDistanceChanged()
        {
            var e = new StepperDistanceChangedEventHandlerArgs();
            DistanceChanged?.Invoke(this, e);
        }

        public event StepperDistanceChangedEventHandler DistanceChanged;
        public event StepperDistanceChangingEventHandler DistanceChanging;
    }
}