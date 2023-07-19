using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShowKeyName
{
    public partial class Form1 : Form
    {
        private Label _label;

        private bool _isDragging = false;

        private Point _dragStartPoint;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var rectangle = new Rectangle(0, 0, this.Width, this.Height);
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int borderRadius = 20; // 自定义圆角半径

            // 绘制圆角矩形路径
            path.AddArc(rectangle.X, rectangle.Y, borderRadius * 2, borderRadius * 2, 180, 90);
            path.AddLine(rectangle.X + borderRadius, rectangle.Y, rectangle.Right - borderRadius, rectangle.Y);
            path.AddArc(rectangle.X + rectangle.Width - borderRadius * 2, rectangle.Y, borderRadius * 2, borderRadius * 2, 270, 90);
            path.AddLine(rectangle.Right, rectangle.Y + borderRadius, rectangle.Right, rectangle.Bottom - borderRadius);
            path.AddArc(rectangle.X + rectangle.Width - borderRadius * 2, rectangle.Y + rectangle.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 0, 90);
            path.AddLine(rectangle.Right - borderRadius, rectangle.Bottom, rectangle.X + borderRadius, rectangle.Bottom);
            path.AddArc(rectangle.X, rectangle.Y + rectangle.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 90, 90);
            path.AddLine(rectangle.X, rectangle.Bottom - borderRadius, rectangle.X, rectangle.Y + borderRadius);

            this.Region = new System.Drawing.Region(path);
        }
        public Form1()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.StartPosition = FormStartPosition.CenterParent;
            // 设置窗体为无边框样式
            this.FormBorderStyle = FormBorderStyle.None;

            // 设置窗体背景颜色并调整透明度
            this.BackColor = Color.Black;
            this.Opacity = 0.6;

            // 设置窗体的大小
            this.Size = new Size(600, 70);

            // 获取屏幕的工作区域大小
            var workingArea = Screen.GetWorkingArea(this);

            // 计算窗口的位置
            int x = (workingArea.Width - this.Width) / 2; // 窗口水平居中
            int y = workingArea.Height - this.Height - 100;    // 窗口垂直位于屏幕底部

            // 设置窗口位置
            this.Location = new Point(x, y);

            _label = new Label
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Size = this.Size,
                ForeColor = Color.GhostWhite,
                Font = new Font("Microsoft YaHei UI", 15f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,

            };
            // 添加鼠标事件处理程序
            _label.MouseDown += (s, e) =>
            {
                // 记录鼠标按下时的位置和状态
                _isDragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
            };
            _label.MouseMove += (s, e) =>
            {
                if (_isDragging)
                {
                    // 根据鼠标移动的距离更新窗体的位置
                    Point newPoint = PointToScreen(new Point(e.X, e.Y));
                    Location = new Point(newPoint.X - _dragStartPoint.X, newPoint.Y - _dragStartPoint.Y);
                }
            };
            _label.MouseUp += (s, e) =>
            { // 重置鼠标状态
                _isDragging = false;
            };

            // 创建上下文菜单
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            // 创建菜单项并添加到上下文菜单
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem("退出");
            menuItem1.Click += (s, e) =>
            {
                Stop();
                Application.Exit();
            };

            contextMenuStrip.Items.Add(menuItem1);

            _label.ContextMenuStrip = contextMenuStrip;

            Controls.Add(_label);

            // 设置窗体为顶层窗口
            TopMost = true;

            this.Load += (s, e) =>
            {
                Start();
            };
            this.FormClosing += (s, e) =>
            {
                Stop();
            };
        }

        private readonly Dictionary<Keys, string> _formatKeyDic = new Dictionary<Keys, string>(){
            { Keys.Left,"←" },
            { Keys.Right,"→" },
            { Keys.Up,"↑" },
            { Keys.Down,"↓" },
            { Keys.LShiftKey,"Shift" },
            { Keys.RShiftKey,"RShift" },
            { Keys.LControlKey,"Ctrl" },
            { Keys.RControlKey,"RCtrl" },
            { Keys.LMenu,"Alt" },
            { Keys.RMenu,"RAlt" },
            { Keys.Escape,"Esc" },
            { Keys.Oemcomma,"," },
            { Keys.OemPeriod,"." },
            { Keys.OemQuestion,"/" },
            { Keys.Capital,"CapsLock" },
            { Keys.Oem5,"\\" },
            { Keys.OemOpenBrackets,"[" },
            { Keys.Oem6,"]" },
            { Keys.Oem1,";" },
            { Keys.Oem7,"'" },
            { Keys.Oemtilde,"`" },
            { Keys.OemMinus,"-" },
            { Keys.Oemplus,"=" },
            { Keys.Return,"Enter" },
        };

        enum MyKeyTypeEnum
        {
            Mouse,
            MouseWheel,
            Keyboard,

        }


        List<string> _keyNameList = new List<string>();

        enum MouseWheelEnum
        {
            ScrolUp,
            ScrollDown
        }
        enum MouseEnum
        {
            /// <summary>
            /// left button
            /// </summary>
            LButton,
            /// <summary>
            /// right button
            /// </summary>
            RButton,
            /// <summary>
            /// side button top
            /// </summary>
            SButtonT,
            /// <summary>
            /// side button bottom
            /// </summary>
            SButtonB
        }

        /// <summary>
        /// 处理按键相关
        /// </summary>
        private void HandleKey(MyKeyTypeEnum keyTypeEnum, int keyCode)
        {
            string keyName = "";

            if (keyTypeEnum == MyKeyTypeEnum.Keyboard)
            {
                var keyEnum = (Keys)keyCode;

                _formatKeyDic.TryGetValue(keyEnum, out string code);

                // 没有匹配项,给一个默认值
                if (string.IsNullOrEmpty(code))
                {
                    keyName = keyEnum.ToString();

                    // 处理 数字键
                    if (keyName.Length == 2 && keyName.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                    {
                        keyName = keyName[1].ToString();
                    }
                    else
                    {

                        if (keyName.Length == 1)
                        {
                            if (Control.IsKeyLocked(Keys.CapsLock)) // 处理大小写
                            {
                                keyName = keyName.ToUpper();
                            }
                            else
                            {
                                keyName = keyName.ToLower();
                            }
                        }
                    }
                }
                else
                {
                    keyName = code;
                }
            }
            else if (keyTypeEnum == MyKeyTypeEnum.Mouse)
            {
                if (keyCode == (int)MouseEnum.LButton)
                {
                    keyName = "鼠标左键";
                }
                else if (keyCode == (int)MouseEnum.RButton)
                {
                    keyName = "鼠标右键";
                }
                else if (keyCode == (int)MouseEnum.SButtonT)
                {
                    keyName = "鼠标上侧键";
                }
                else if (keyCode == (int)MouseEnum.SButtonB)
                {
                    keyName = "鼠标下侧键";
                }
            }
            else if (keyTypeEnum == MyKeyTypeEnum.MouseWheel)
            {
                if (keyCode == (int)MouseWheelEnum.ScrolUp)
                {
                    keyName = "ScrolUp";
                }
                else if (keyCode == (int)MouseWheelEnum.ScrollDown)
                {
                    keyName = "ScrollDown";
                }
            }

            ShowKey(keyName);
        }

        private void ShowKey(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return;
            }

            // shift + 组合键
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                keyName = "⇧" + keyName;
            }


            // 超长,则清空
            if (_label.Text.Length > 15 && IsTextOverflow(_label))
            {
                _keyNameList.Clear();
            }

            if (_keyNameList.Count > 0)
            {
                var endItem = _keyNameList[_keyNameList.Count - 1];

                if (endItem.StartsWith("x") && endItem.Length >= 2)
                {
                    if (_keyNameList[_keyNameList.Count - 2] == keyName)
                    {
                        var endItemNum = endItem.Substring(1);
                        var num = Convert.ToInt32(endItemNum);
                        _keyNameList[_keyNameList.Count - 1] = "x" + (num + 1);
                    }
                    else
                    {
                        _keyNameList.Add(keyName);
                    }
                }
                else
                {
                    if (endItem == keyName)
                    {
                        _keyNameList.Add("x2");

                    }
                    else
                    {
                        _keyNameList.Add(keyName);
                    }
                }

            }
            else
            {
                _keyNameList.Add(keyName);
            }


            _label.Text = string.Join(" ", _keyNameList);
        }

        /// <summary>
        /// 判断文字是否超出了控件的显示范围
        /// </summary>
        private bool IsTextOverflow(Label label)
        {
            return TextRenderer.MeasureText(label.Text, label.Font).Width >= label.Width - 150;
        }

        // ******************************************************************
        // ****************************** Hook ******************************
        // ******************************************************************


        // 可以使用 Vanara 调用 windows api, 但程序体积会变大

        // 全局键盘钩子的委托回调函数
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        // 导入 Windows API 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        // 全局键盘钩子
        private IntPtr _keyBoardHookHandle = IntPtr.Zero;
        private IntPtr _mouseHookHandle = IntPtr.Zero;
        private static LowLevelKeyboardProc _hookCallbackFunc = null;

        const int WM_KEYDOWN = 0x0100;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_XBUTTONDOWN = 0x020B;

        const int WH_KEYBOARD_LL = 13;
        const int WH_MOUSE_LL = 14;

        public struct MOUSEINPUT
        {
            public int dx;

            public int dy;

            public int mouseData;

            //public MOUSEEVENTF dwFlags;

            public uint time;

            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 开始全局键盘钩子
        /// </summary>
        public void Start()
        {
            if (_keyBoardHookHandle == IntPtr.Zero)
            {

                var hMod = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                LowLevelKeyboardProc myFunction = (int nCode, IntPtr wParam, IntPtr lParam) =>
                {
                    if (nCode >= 0)
                    {
                        switch ((int)wParam)
                        {
                            case WM_KEYDOWN: // 键盘按下
                                HandleKey(MyKeyTypeEnum.Keyboard, Marshal.ReadInt32(lParam));
                                break;

                            case WM_MOUSEWHEEL: // 鼠标滚轮
                                var mouseWheelData = Marshal.PtrToStructure<MOUSEINPUT>(lParam);

                                HandleKey(MyKeyTypeEnum.MouseWheel, mouseWheelData.mouseData > 0 ? (int)MouseWheelEnum.ScrolUp : (int)MouseWheelEnum.ScrollDown);
                                break;
                            case WM_MOUSEMOVE: // 鼠标移动
                                break;

                            case WM_LBUTTONDOWN: // 鼠标左键按下
                                HandleKey(MyKeyTypeEnum.Mouse, (int)MouseEnum.LButton);
                                break;
                            case WM_RBUTTONDOWN: // 鼠标右键按下
                                HandleKey(MyKeyTypeEnum.Mouse, (int)MouseEnum.RButton);
                                break;
                            case WM_XBUTTONDOWN: // 鼠标侧键按下
                                var xButtonData = Marshal.PtrToStructure<MOUSEINPUT>(lParam);

                                var xButtonType = Helper.GetUnsignedHWORD((uint)xButtonData.mouseData);

                                
                                if (xButtonType == 1)// 1 下 
                                {
                                    HandleKey(MyKeyTypeEnum.Mouse, (int)MouseEnum.SButtonB);
                                }
                                else if (xButtonType == 2)// 2 上
                                {
                                    HandleKey(MyKeyTypeEnum.Mouse, (int)MouseEnum.SButtonT);
                                }

                                break;

                            default:
                                break;
                        }

                    }

                    return CallNextHookEx(_keyBoardHookHandle, nCode, wParam, lParam);
                };

                // 保存到 静态变量, 防止被 GC 回收, ( 回收后 c++ 层会空指针 )
                _hookCallbackFunc = myFunction;

                // 设置 hooks
                _keyBoardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookCallbackFunc, hMod, 0);
                _mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, _hookCallbackFunc, hMod, 0);
            }
        }

        /// <summary>
        /// 停止全局键盘钩子
        /// </summary>
        public void Stop()
        {
            // 如果有钩子句柄，则卸载钩子
            if (_keyBoardHookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyBoardHookHandle);
                _keyBoardHookHandle = IntPtr.Zero;
                if (_hookCallbackFunc != null)
                {
                    // 释放静态变量, 允许 GC 回收
                    _hookCallbackFunc = null;
                }
            }

            if (_mouseHookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookHandle);
                _mouseHookHandle = IntPtr.Zero;
                if (_hookCallbackFunc != null)
                {
                    // 释放静态变量, 允许 GC 回收
                    _hookCallbackFunc = null;
                }
            }
        }
    }

    static class Helper
    {
        /// <summary>
        /// Retrieves the High Order Word from a 32-bit unsigned integer and returns it as a 16-bit signed integer.
        /// </summary>
        /// <param name="_uint">The 32-bit unsigned integer where the High Order Word is stored.</param>
        /// <returns>Returns a 16 bit signed integer as the High Order Word.</returns>
        public static short GetSignedHWORD(uint _uint)
        {
            return (short)((_uint >> 16) & 0xFFFF); //Shift the leftmost 16 bits to the right and mask the leftmost 16 bits. Get the rightmost 16 bits by casting the whole thing to a short value. (16 bit integer)
        }
        /// <summary>
        /// Retrieves the Low Order Word from a 32-bit unsigned integer and returns it as a 16-bit signed integer.
        /// </summary>
        /// <param name="_uint">The 32-bit unsigned integer where the Low Order Word is stored.</param>
        /// <returns>Returns a 16 bit signed integer as the Low Order Word.</returns>
        public static short GetSignedLWORD(uint _uint)
        {
            return (short)(_uint & 0xFFFF);  //No need to shift the bits since we just need the rightmost 16 bits. All we have to do is mask the leftmost 16 bits and cast the whole thing to a short value. (16 bit integer.)
        }

        /// <summary>
        /// Retrieves the High Order Word from a 32-bit unsigned integer and returns it as a 16-bit unsigned integer.
        /// </summary>
        /// <param name="_uint">The 32-bit unsigned integer where the High Order Word is stored.</param>
        /// <returns>Returns a 16 bit unsigned integer as the High Order Word.</returns>
        public static ushort GetUnsignedHWORD(uint _uint)
        {
            return (ushort)((_uint >> 16) & 0xFFFF);
        }

        /// <summary>
        /// Retrieves the Low Order Word from a 32-bit unsigned integer and returns it as a 16-bit unsigned integer.
        /// </summary>
        /// <param name="_uint">The 32-bit unsigned integer where the Low Order Word is stored.</param>
        /// <returns>Returns a 16 bit unsigned integer as the Low Order Word.</returns>
        public static ushort GetUnsignedLWORD(uint _uint)
        {
            return (ushort)(_uint & 0xFFFF);
        }
    }

}
