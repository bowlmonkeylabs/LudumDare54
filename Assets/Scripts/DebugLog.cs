using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BML.Scripts {
    public class DebugLog : MonoBehaviour
    {
        private enum LogLevel {
            Debug,
            Error,
            Warning
        }

        [SerializeField] private LogLevel _logLevel = LogLevel.Debug;
        [SerializeField] private string _message;

        private void Log(string message) {
            if(_logLevel == LogLevel.Debug) {
                Debug.Log(message);
            }
            if(_logLevel == LogLevel.Error) {
                Debug.LogError(message);
            }
            if(_logLevel == LogLevel.Warning) {
                Debug.LogWarning(message);
            }
        }

        public void DoLog() {
            this.Log(this._message);
        }

        public void DoLog(string message) {
            this.Log(message);
        }
    }
}
