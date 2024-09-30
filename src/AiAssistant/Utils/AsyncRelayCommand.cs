using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AiAssistant.Utils
{
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private long _isExecuting;

        public event EventHandler? CanExecuteChanged;
        public AsyncRelayCommand(Func<Task> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = execute;
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            if (Interlocked.Read(ref _isExecuting) != 0)
                return false;

            return _canExecute?.Invoke() != false;
        }

        /// <inheritdoc/>
        public async void Execute(object? parameter)
        {
            Interlocked.Exchange(ref _isExecuting, 1);

            try
            {
                await _execute();
            }
            finally
            {
                Interlocked.Exchange(ref _isExecuting, 0);
            }
        }
    }
}
