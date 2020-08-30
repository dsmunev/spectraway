using System;
using ArduinoDriver;
using ArduinoDriver.SerialProtocol;
using ArduinoUploader.Hardware;
using NLog;

namespace SpectraWay.Device.Stepper
{
    public class Stepper:IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ArduinoDriver.ArduinoDriver _driver;


        private Stepper()
        {

            try
            {
                _driver = new ArduinoDriver.ArduinoDriver(ArduinoModel.UnoR3, "COM3", true);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public Stepper(byte number_of_steps, byte motor_pin_1, byte motor_pin_2):this()
        {
            this.step_number = 0;    // which step the motor is on
            this.direction = 0;      // motor direction
            this.last_step_time = 0; // time stamp in us of the last step taken
            this.number_of_steps = number_of_steps; // total number of steps for this motor

            // Arduino pins for the motor control connection:
            this.motor_pin_1 = motor_pin_1;
            this.motor_pin_2 = motor_pin_2;

            // setup the pins on the microcontroller:
            //pinMode(this.motor_pin_1, OUTPUT);
            //pinMode(this.motor_pin_2, OUTPUT);
            _driver.Send(new PinModeRequest(this.motor_pin_1, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_2, PinMode.Output));

            // When there are only 2 pins, set the others to 0:
            this.motor_pin_3 = 0;
            this.motor_pin_4 = 0;
            this.motor_pin_5 = 0;

            // pin_count is used by the stepMotor() method:
            this.pin_count = 2;
        }

        public Stepper(byte number_of_steps, byte motor_pin_1, byte motor_pin_2,
            byte motor_pin_3, byte motor_pin_4) : this()
        {
            this.step_number = 0;    // which step the motor is on
            this.direction = 0;      // motor direction
            this.last_step_time = 0; // time stamp in us of the last step taken
            this.number_of_steps = number_of_steps; // total number of steps for this motor

            // Arduino pins for the motor control connection:
            this.motor_pin_1 = motor_pin_1;
            this.motor_pin_2 = motor_pin_2;
            this.motor_pin_3 = motor_pin_3;
            this.motor_pin_4 = motor_pin_4;

            // setup the pins on the microcontroller:
            //pinMode(this.motor_pin_1, OUTPUT);
            //pinMode(this.motor_pin_2, OUTPUT);
            //pinMode(this.motor_pin_3, OUTPUT);
            //pinMode(this.motor_pin_4, OUTPUT);
            _driver.Send(new PinModeRequest(this.motor_pin_1, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_2, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_3, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_4, PinMode.Output));

            // When there are 4 pins, set the others to 0:
            this.motor_pin_5 = 0;

            // pin_count is used by the stepMotor() method:
            this.pin_count = 4;
        }

        public Stepper(byte number_of_steps, byte motor_pin_1, byte motor_pin_2,
            byte motor_pin_3, byte motor_pin_4,
            byte motor_pin_5) : this()
        {
            this.step_number = 0;    // which step the motor is on
            this.direction = 0;      // motor direction
            this.last_step_time = 0; // time stamp in us of the last step taken
            this.number_of_steps = number_of_steps; // total number of steps for this motor

            // Arduino pins for the motor control connection:
            this.motor_pin_1 = motor_pin_1;
            this.motor_pin_2 = motor_pin_2;
            this.motor_pin_3 = motor_pin_3;
            this.motor_pin_4 = motor_pin_4;
            this.motor_pin_5 = motor_pin_5;

            // setup the pins on the microcontroller:
            //pinMode(this.motor_pin_1, OUTPUT);
            //pinMode(this.motor_pin_2, OUTPUT);
            //pinMode(this.motor_pin_3, OUTPUT);
            //pinMode(this.motor_pin_4, OUTPUT);
            //pinMode(this.motor_pin_5, OUTPUT);
            _driver.Send(new PinModeRequest(this.motor_pin_1, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_2, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_3, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_4, PinMode.Output));
            _driver.Send(new PinModeRequest(this.motor_pin_5, PinMode.Output));

            // pin_count is used by the stepMotor() method:
            this.pin_count = 5;
        }

        // speed setter method:
        public void setSpeed(long whatSpeed)
        {
            this.step_delay = 60L * 1000L * 1000L / this.number_of_steps / whatSpeed;
        }

        // mover method:
        public void step(int steps_to_move)
        {
            int steps_left = Math.Abs(steps_to_move);  // how many steps to take

            // determine direction based on whether steps_to_mode is + or -:
            if (steps_to_move > 0) { this.direction = 1; }
            if (steps_to_move < 0) { this.direction = 0; }


            // decrement the number of steps, moving one step each time:
            while (steps_left > 0)
            {
                long now = DateTime.UtcNow.Ticks;
                // move only if the appropriate delay has passed:
                if (now - this.last_step_time >= this.step_delay)
                {
                    // get the timeStamp of when you stepped:
                    this.last_step_time = now;
                    // increment or decrement the step number,
                    // depending on direction:
                    if (this.direction == 1)
                    {
                        this.step_number++;
                        if (this.step_number == this.number_of_steps)
                        {
                            this.step_number = 0;
                        }
                    }
                    else
                    {
                        if (this.step_number == 0)
                        {
                            this.step_number = this.number_of_steps;
                        }
                        this.step_number--;
                    }
                    // decrement the steps left:
                    steps_left--;
                    // step the motor to step number 0, 1, ..., {3 or 10}
                    if (this.pin_count == 5)
                        stepMotor(this.step_number % 10);
                    else
                        stepMotor(this.step_number % 4);
                }
            }
        }

        public int version()
        {
            return 5;
        }

        private DigitalValue HIGH = DigitalValue.High;
        private DigitalValue LOW = DigitalValue.Low;

        void stepMotor(int this_step)
        {
            if (this.pin_count == 2)
            {
                switch (this_step)
                {
                    case 0:  // 01
                        //_driver.Send(new DigitalWriteRequest(13, DigitalValue.High));
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        break;
                    case 1:  // 11
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        break;
                    case 2:  // 10
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        break;
                    case 3:  // 00
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        break;
                }
            }
            if (this.pin_count == 4)
            {
                switch (this_step)
                {
                    case 0:  // 1010
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        break;
                    case 1:  // 0110
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        break;
                    case 2:  //0101
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        break;
                    case 3:  //1001
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        break;
                }
            }

            if (this.pin_count == 5)
            {
                switch (this_step)
                {
                    case 0:  // 01101
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, HIGH));
                        break;
                    case 1:  // 01001
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, HIGH));
                        break;
                    case 2:  // 01011
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, HIGH));
                        break;
                    case 3:  // 01010
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, LOW));
                        break;
                    case 4:  // 11010
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, LOW));
                        break;
                    case 5:  // 10010
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, LOW));
                        break;
                    case 6:  // 10110
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, LOW));
                        break;
                    case 7:  // 10100
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, LOW));
                        break;
                    case 8:  // 10101
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, HIGH));
                        break;
                    case 9:  // 00101
                        _driver.Send(new DigitalWriteRequest(motor_pin_1, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_2, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_3, HIGH));
                        _driver.Send(new DigitalWriteRequest(motor_pin_4, LOW));
                        _driver.Send(new DigitalWriteRequest(motor_pin_5, HIGH));
                        break;
                }
            }
        }

        int direction;            // Direction of rotation
        long step_delay; // delay between steps, in ms, based on speed
        int number_of_steps;      // total number of steps this motor can take
        int pin_count;            // how many pins are in use.
        int step_number;          // which step the motor is on

        // motor pin numbers:
        byte motor_pin_1;
        byte motor_pin_2;
        byte motor_pin_3;
        byte motor_pin_4;
        byte motor_pin_5;          // Only 5 phase motor

        long last_step_time; // time stamp in us of when the last step was taken
        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}