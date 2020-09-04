using System;
using System.Collections.Generic;
using System.Linq;
using QTMRealTimeSDK.Data;
using UnityEngine;

namespace QualisysRealTime.Unity
{
    internal class QtmCommandHandler
    {
        internal struct RtCommandCompleteTrigger
        {
            internal QTMEvent triggerEvent;
            internal string resultString;
        }

        internal interface ICommand
        {
            Action<bool> OnResult { get; }
            string Command { get; }
            RtCommandCompleteTrigger[] Triggers { get; }
        }

        internal struct CommandResult
        {
            internal ICommand command;
            internal bool success;
            internal string message;
        }

        internal class QtmCommandAwaiter
        {
            ICommand command;
            List<QTMEvent> events = new List<QTMEvent>();
            string resultString = "";
            internal bool IsAwaiting { get; private set; } = false;

            internal void Await(ICommand command)
            {
                this.command = command;
                this.resultString = string.Empty;
                this.events.Clear();
                this.IsAwaiting = true;
            }

            internal void AppendEvent(QTMEvent qtmEvent)
            {
                Debug.Log(command.GetType().Name + " response event: " + qtmEvent.ToString());
                events.Add(qtmEvent);
            }
            internal void SetResultString(string resultString)
            {
                Debug.Log(command.GetType().Name + " response: " + resultString);
                this.resultString = resultString;
            }

            internal bool TryGetResult(out CommandResult result)
            {

                var triggers = command.Triggers;
                for (int i = 0; i < triggers.Length; ++i)
                {
                    if (triggers[i].resultString.StartsWith(this.resultString)
                        && (triggers[i].triggerEvent == QTMEvent.EventNone || events.Contains(triggers[i].triggerEvent)))
                    {
                        result = new CommandResult()
                        {
                            command = command,
                            success = true,
                            message = "",
                        };
                        IsAwaiting = false;
                        return true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.resultString)
                            && !triggers.Any(x => x.resultString == this.resultString))
                        {
                            result = new CommandResult()
                            {
                                command = command,
                                success = false,
                                message = this.resultString,
                            };
                            IsAwaiting = false;
                            return true;
                        }
                    }
                }
                result = default(CommandResult);
                return false;
            }

            internal void CancelAwait()
            {
                IsAwaiting = false;
            }
        }


        internal struct StartCapture : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]{
                 new RtCommandCompleteTrigger{ resultString = "Starting measurement", triggerEvent = QTMEvent.EventCaptureStarted },
                 new RtCommandCompleteTrigger{ resultString = "Measurement is already running", triggerEvent = QTMEvent.EventNone },
            };
            public string Command { get => "Start"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            public StartCapture(Action<bool> onResult)
            {
                this.onResult = onResult;
            }
        }
        internal struct StartRtFromFile : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]{
                 new RtCommandCompleteTrigger{ resultString = "Starting measurement", triggerEvent = QTMEvent.EventCaptureStarted },
                 new RtCommandCompleteTrigger{ resultString = "Measurement is already running", triggerEvent = QTMEvent.EventNone },
            };
            public string Command { get => "Start rtfromfile"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            public StartRtFromFile(Action<bool> onResult)
            {
                this.onResult = onResult;
            }
        }

        internal struct Stop : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]{
                 new RtCommandCompleteTrigger{ resultString = "Stopping measurement", triggerEvent = QTMEvent.EventNone },
            };
            public string Command { get => "Stop"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            public Stop(Action<bool> onResult)
            {
                this.onResult = onResult;
            }
        }


        internal struct Close : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]{
                 new RtCommandCompleteTrigger{ resultString = "Closing connection", triggerEvent = QTMEvent.EventConnectionClosed },
                 new RtCommandCompleteTrigger{ resultString = "No connection to close", triggerEvent = QTMEvent.EventNone },
                 new RtCommandCompleteTrigger{ resultString = "File closed", triggerEvent = QTMEvent.EventNone },
            };
            public string Command { get => "Close"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }
            Action<bool> onResult;
            public Close(Action<bool> onResult)
            {
                this.onResult = onResult;
            }
        }


        internal struct New : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]{
                new RtCommandCompleteTrigger{ resultString = "Creating new connection", triggerEvent = QTMEvent.EventConnected },
                new RtCommandCompleteTrigger{ resultString = "Already connected", triggerEvent = QTMEvent.EventNone },
            };
            public string Command { get => "New"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onComplete; }
            Action<bool> onComplete;
            public New(Action<bool> onResult)
            {
                this.onComplete = onResult;
            }
        }

        internal struct Save : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]
            {
                new RtCommandCompleteTrigger{ resultString = "Measurement saved", triggerEvent = QTMEvent.EventCaptureSaved },
            };

            public string Command { get => "save " + fileName + " overwrite"; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            string fileName;
            public Save(string fileName, Action<bool> onResult)
            {
                this.onResult = onResult;
                this.fileName = fileName;
            }
        }

        internal struct TakeControl : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]
            {
                new RtCommandCompleteTrigger{ resultString = "You are now master", triggerEvent = QTMEvent.EventNone },
            };

            public string Command { get => "TakeControl " + password; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            string password;
            public TakeControl(string password, Action<bool> onResult)
            {
                this.onResult = onResult;
                this.password = password;
            }
        }

        internal struct ReleaseControl : ICommand
        {
            static RtCommandCompleteTrigger[] trigger_cache = new RtCommandCompleteTrigger[]
            {
                new RtCommandCompleteTrigger{ resultString = "You are now a regular client", triggerEvent = QTMEvent.EventNone },
            };

            public string Command { get => "releaseControl "; }
            public RtCommandCompleteTrigger[] Triggers { get => trigger_cache; }
            public Action<bool> OnResult { get => onResult; }

            Action<bool> onResult;
            public ReleaseControl(Action<bool> onResult)
            {
                this.onResult = onResult;
            }
        }

    }
}
