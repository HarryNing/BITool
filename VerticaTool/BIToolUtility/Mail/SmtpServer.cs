using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BIToolUtility.Common;

namespace BIToolUtility.Mail
{
    public class SmtpServer : INotifyPropertyChanged
    {
        private string _server;
        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
                this.OnPropertyChanged(() => this.Server);
            }
        }

        private int _port;
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                this.OnPropertyChanged(() => this.Port);
            }
        }

        private string _user;
        public string User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                this.OnPropertyChanged(() => this.User);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                this.OnPropertyChanged(() => this.Password);
            }
        }

        private string _pickupDirectoryLocation;
        public string PickupDirectoryLocation
        {
            get
            {
                return _pickupDirectoryLocation;
            }
            set
            {
                _pickupDirectoryLocation = value;
                this.OnPropertyChanged(() => this.PickupDirectoryLocation);
            }
        }

        private string _exceptionFrom;
        public string ExceptionFrom
        {
            get
            {
                return _exceptionFrom;
            }
            set
            {
                _exceptionFrom = value;
                this.OnPropertyChanged(() => this.ExceptionFrom);
            }
        }

        private string _exceptionTo;
        public string ExceptionTo
        {
            get
            {
                return _exceptionTo;
            }
            set
            {
                _exceptionTo = value;
                this.OnPropertyChanged(() => this.ExceptionTo);
            }
        }

        private bool _usePickupDirectoryLocation;
        public bool UsePickupDirectoryLocation
        {
            get
            {
                return _usePickupDirectoryLocation;
            }
            set
            {
                _usePickupDirectoryLocation = value;
                this.OnPropertyChanged(() => this.UsePickupDirectoryLocation);
            }
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(this.Server)
                 && !string.IsNullOrEmpty(this.User)
                 && !string.IsNullOrEmpty(this.Password)
                 && !string.IsNullOrEmpty(this.PickupDirectoryLocation);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(Expression<Func<object>> property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(BindingHelper.Name(property)));
            }
        }
    }
}
