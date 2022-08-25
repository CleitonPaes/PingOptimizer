using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PingOptimizer
{
    public partial class Form1 : Form
    {
        private readonly string MYNIC = ActiveNetworkInterface().ToString();

        public Form1()
        {
            InitializeComponent();
            Lock();
        }

        public static IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .Where(a => Array.FindIndex(a.GetAddressBytes(), b => b != 0) >= 0)
                .FirstOrDefault();
        }

        public static string ActiveNetworkInterface()
        {
            UdpClient u = new(GetDefaultGateway().ToString(), 1);
            IPAddress localAddr = (u.Client.LocalEndPoint as IPEndPoint).Address;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = nic.GetIPProperties();
                foreach (UnicastIPAddressInformation addrinfo in ipProps.UnicastAddresses)
                {
                    if (localAddr.Equals(addrinfo.Address))
                    {
                        return nic.Id;
                    }
                }
            }
            return "Adaptador Não Encontado";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            Lock();
            if (RadioOptimal.Checked)
            {
                Optimize();
            }
            if (RadioDefault.Checked)
            {
                Default();
            }
            if (RadioCustom.Checked)
            {
                Custom();
            }
        }

        private void Lock()
        {
            foreach (Control all in Controls.OfType<TextBox>())
            {
                all.Enabled = false;
            }

            foreach (Control all in Controls.OfType<ComboBox>())
            {
                all.Enabled = false;
            }
        }

        private void Optimize()
        {
            var key = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
            key.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
            key.Close();

            var key2 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
            key2.SetValue("TcpDelAckTicks", 0, RegistryValueKind.DWord);
            key2.Close();

            var key3 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
            key3.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
            key3.Close();

            var key4 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\MSMQ\\Parameters");
            key4.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
            key4.Close();

            var key5 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
            key5.SetValue("NetworkThrottlingIndex", -1, RegistryValueKind.DWord);
            key5.Close();

            var key6 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
            key6.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
            key6.Close();

            var key7 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            key7.SetValue("DefaultTTL", 64, RegistryValueKind.DWord);
            key7.Close();

            var key8 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            key8.SetValue("Tcp1323Opts", 0, RegistryValueKind.DWord);
            key8.Close();

            var key9 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            key9.SetValue("MaxUserPort", 65534, RegistryValueKind.DWord);
            key9.Close();

            var key10 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
            key10.SetValue("TcpTimedWaitDelay", 30, RegistryValueKind.DWord);
            key10.Close();

            var key11 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key11.SetValue("DnsPriority", 6, RegistryValueKind.DWord);
            key11.Close();

            var key12 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key12.SetValue("HostsPriority", 5, RegistryValueKind.DWord);
            key12.Close();

            var key13 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key13.SetValue("LocalPriority", 4, RegistryValueKind.DWord);
            key13.Close();

            var key14 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key14.SetValue("NetbtPriority", 7, RegistryValueKind.DWord);
            key14.Close();

            var key15 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\Psched");
            key15.SetValue("NonBestEffortLimit", 0, RegistryValueKind.DWord);
            key15.Close();

            var key16 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\QoS");
            key16.SetValue("Do not use NLA", 1, RegistryValueKind.String);
            key16.Close();

            var key17 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management");
            key17.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord);
            key17.Close();

            var key18 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\LanmanServer\\Parameters");
            key18.SetValue("Size", 3, RegistryValueKind.DWord);
            key18.Close();


            Process cmd = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd.exe",
            };

            cmd.StartInfo = startInfo;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;


            startInfo.Arguments = "/C netsh int tcp set global ecncapability=enabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global rsc=disabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global autotuning=normal";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global autotuningl=normal";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set heuristics disabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set supplemental Internet congestionprovider=ctcp";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global rss=enabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global MaxSynRetransmissions=2";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global nonsackrttresiliency=disabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global initialRto=2000";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global timestamps=disabled";
            cmd.Start();
            cmd.WaitForExit();

            Reboot();
        }

        private void Default()
        {
            var key5 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
            key5.SetValue("NetworkThrottlingIndex", 10, RegistryValueKind.DWord);
            key5.Close();

            var key6 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
            key6.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
            key6.Close();

            var key11 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key11.SetValue("DnsPriority", 2000, RegistryValueKind.DWord);
            key11.Close();

            var key12 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key12.SetValue("HostsPriority", 500, RegistryValueKind.DWord);
            key12.Close();

            var key13 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key13.SetValue("LocalPriority", 499, RegistryValueKind.DWord);
            key13.Close();

            var key14 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
            key14.SetValue("NetbtPriority", 2001, RegistryValueKind.DWord);
            key14.Close();

            var key17 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management");
            key17.SetValue("LargeSystemCache", 0, RegistryValueKind.DWord);
            key17.Close();

            var key18 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\LanmanServer\\Parameters");
            key18.SetValue("Size", 1, RegistryValueKind.DWord);
            key18.Close();

            Process cmd = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd.exe",
            };

            cmd.StartInfo = startInfo;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            startInfo.Arguments = "/C netsh int tcp set global ecncapability=default";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global rsc=enabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global autotuning=normal";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global autotuningl=normal";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set heuristics default";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set supplemental Internet congestionprovider=CUBIC";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global rss=enabled";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh interface tcp set global MaxSynRetransmissions=2";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global nonsackrttresiliency=default";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global initialRto=3000";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C netsh int tcp set global timestamps=default";
            cmd.Start();
            cmd.WaitForExit();

            // deleting keys that did not previously exist

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces`) do ( reg delete  %i /v \"TcpAckFrequency\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces`) do ( reg delete  %i /v \"TcpDelAckTicks\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces`) do ( reg delete  %i /v \"TCPNoDelay\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSMQ\\Parameters`) do ( reg delete  %i /v \"TCPNoDelay\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters`) do ( reg delete  %i /v \"DefaultTTL\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters`) do ( reg delete  %i /v \"MaxUserPort\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters`) do ( reg delete  %i /v \"tcptimewaitdelay\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters`) do ( reg delete  %i /v \"Tcp1323Opts\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Psched`) do ( reg delete  %i /v \"NonBestEffortLimit\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            startInfo.Arguments = "/C for /f \"usebackq\" %i in (`reg query HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\QoS`) do ( reg delete  %i /v \"Do not use NLA\" /f )";
            cmd.Start();
            cmd.WaitForExit();

            Reboot();
        }

        private void Custom()
        {
            //Check error
            if (Ttl.Text != "")
            {
                if (Convert.ToInt32(Ttl.Text) < 32 | Convert.ToInt32(Ttl.Text) > 128)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 32 and 128 on Time to Live", "PingOptimizer");
                    return;
                }
            }

            if (Dnspriority.Text != "") {
                if (Convert.ToInt32(Dnspriority.Text) < 0 | Convert.ToInt32(Dnspriority.Text) > 65535)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 0 and 65535 on DnsPriority", "PingOptimizer");
                    return;
                }
            }

            if (Hostpriority.Text != "") { 
                if (Convert.ToInt32(Hostpriority.Text) < 0 | Convert.ToInt32(Hostpriority.Text) > 65535)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 0 and 65535 on HostPriority", "PingOptimizer");
                    return;
                }
            }

            if (Localpriority.Text != "")
            {
                if (Convert.ToInt32(Localpriority.Text) < 0 | Convert.ToInt32(Localpriority.Text) > 65535)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 0 and 65535 on LocalPriority", "PingOptimizer");
                    return;
                }
            }

            if (Netbtpriority.Text != "")
            {
                if (Convert.ToInt32(Netbtpriority.Text) < 0 | Convert.ToInt32(Netbtpriority.Text) > 65535)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 0 and 65535 on NetbtPriority", "PingOptimizer");
                    return;
                }
            }

            if (Maxuserport.Text != "") { 
                if (Convert.ToInt32(Maxuserport.Text) < 16384 | Convert.ToInt32(Maxuserport.Text) > 65535)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 16384 and 65535 on MaxUserPort", "PingOptimizer");
                    return;
                }
            }

            if (Tcptimedwaitdelay.Text != "") {
                if (Convert.ToInt32(Tcptimedwaitdelay.Text) < 1 | Convert.ToInt32(Tcptimedwaitdelay.Text) > 3600)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 1 and 3600 on TcpTimedWaitDelay", "PingOptimizer");
                    return;
                }
            }

            if (Nonbesteffortlimit.Text != "") {
                if (Convert.ToInt32(Nonbesteffortlimit.Text) < 0 | Convert.ToInt32(Nonbesteffortlimit.Text) > 100)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 0 and 100 on NonBestEffortLimit", "PingOptimizer");
                    return;
                }
            }

            if (Initialrto.Text != "") {
                if (Convert.ToInt32(Initialrto.Text) < 100 | Convert.ToInt32(Initialrto.Text) > 30000)
                {
                    DialogResult = MessageBox.Show("Insert a value betwen 100 and 30000 on Initial RTO", "PingOptimizer");
                    return;
                }
            }

            Process cmd = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd.exe",
            };

            cmd.StartInfo = startInfo;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            if (Provider.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set supplemental Internet congestionprovider=" + Provider.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Tuning.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh interface tcp set global autotuning=" + Tuning.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Heuristics.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set heuristics " + Heuristics.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Timestamps.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set global timestamps=" + Timestamps.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }
            if (Timestamps.SelectedItem.ToString() != "enabled")
            {
                var key8 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
                key8.SetValue("Tcp1323Opts", 1, RegistryValueKind.DWord);
                key8.Close();
            }
            if (Timestamps.SelectedItem.ToString() == "disabled" | Timestamps.SelectedItem.ToString() == "default")
            {
                var key8 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
                key8.SetValue("Tcp1323Opts", 0, RegistryValueKind.DWord);
                key8.Close();
            }

            if (Ttl.Text.Length >= 1)
            {
                var key7 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
                key7.SetValue("DefaultTTL", Ttl.Text, RegistryValueKind.DWord);
                key7.Close();
            }

            if (Tcpackfrequency.SelectedItem.ToString() != "")
            {
                var key = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
                key.SetValue("TcpAckFrequency", Tcpackfrequency.SelectedItem, RegistryValueKind.DWord);
                key.Close();
            }

            if (Tcpdelackticks.SelectedItem.ToString() != "")
            {
                var key2 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
                key2.SetValue("TcpDelAckTicks", Tcpdelackticks.SelectedItem, RegistryValueKind.DWord);
                key2.Close();
            }

            if (Tcpnodelay.SelectedItem.ToString() != "")
            {
                var key3 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\Interfaces\\" + MYNIC);
                key3.SetValue("TCPNoDelay", Tcpnodelay.SelectedItem, RegistryValueKind.DWord);
                key3.Close();

                var key4 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\MSMQ\\Parameters");
                key4.SetValue("TCPNoDelay", Tcpnodelay.SelectedItem, RegistryValueKind.DWord);
                key4.Close();
            }

            if (Networkthrottlingindex.SelectedItem.ToString() != "")
            {
                var key5 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
                key5.SetValue("NetworkThrottlingIndex", Networkthrottlingindex.SelectedItem, RegistryValueKind.DWord);
                key5.Close();
            }

            if (Systemresponsiveness.SelectedItem.ToString() != "")
            {
                var key6 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");
                key6.SetValue("SystemResponsiveness", Systemresponsiveness.SelectedItem, RegistryValueKind.DWord);
                key6.Close();
            }

            if (Ecn.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set global ecncapability=" + Ecn.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Rss.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh interface tcp set global rss=" + Rss.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Rsc.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set global rsc=" + Rsc.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Maxuserport.Text.Length >= 1)
            {
                var key9 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
                key9.SetValue("MaxUserPort", Maxuserport.Text, RegistryValueKind.DWord);
                 key9.Close();
            }

            if (Tcptimedwaitdelay.Text.Length >= 1)
            {
                var key10 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters");
                key10.SetValue("TcpTimedWaitDelay", Tcptimedwaitdelay.Text, RegistryValueKind.DWord);
                key10.Close();
            }

            if (Dnspriority.Text.Length >= 1)
            {
                var key11 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
                 key11.SetValue("DnsPriority", Dnspriority.Text, RegistryValueKind.DWord);
                key11.Close();
            }

            if (Hostpriority.Text.Length >= 1)
            {
                var key12 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
                key12.SetValue("HostsPriority", Hostpriority.Text, RegistryValueKind.DWord);
                key12.Close();
            }

            if (Localpriority.Text.Length >= 1)
            {
                var key13 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
                key13.SetValue("LocalPriority", Localpriority.Text, RegistryValueKind.DWord);
                key13.Close();
            }

            if (Netbtpriority.Text.Length >= 1)
            {
                var key14 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\ServiceProvider");
                key14.SetValue("NetbtPriority", Netbtpriority.Text, RegistryValueKind.DWord);
                key14.Close();
            }

            if (Largesystemcache.SelectedItem.ToString() != "")
            {
                var key17 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management");
                key17.SetValue("LargeSystemCache", Largesystemcache.SelectedItem, RegistryValueKind.DWord);
                key17.Close();
            }

            if (Memoryallocationsize.SelectedItem.ToString() != "")
            {
                var key18 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\LanmanServer\\Parameters");
                key18.SetValue("Size", Memoryallocationsize.SelectedItem, RegistryValueKind.DWord);
                key18.Close();
            }

            if (Nla.SelectedItem.ToString() != "")
            {
                var key16 = Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\QoS");
                key16.SetValue("Do not use NLA", Nla.SelectedItem, RegistryValueKind.String);
                key16.Close();
            }

            if (Nonbesteffortlimit.Text.Length >= 1)
            {
                var key15 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\Psched");
                key15.SetValue("NonBestEffortLimit", Nonbesteffortlimit.Text, RegistryValueKind.DWord);
                key15.Close();
            }

            if (Maxsyn.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh interface tcp set global MaxSynRetransmissions=" + Maxsyn.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Nonsack.SelectedItem.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set global nonsackrttresiliency=" + Nonsack.SelectedItem;
                cmd.Start();
                cmd.WaitForExit();
            }

            if (Initialrto.Text.ToString() != "")
            {
                startInfo.Arguments = "/C netsh int tcp set global initialRto=" + Initialrto.Text;
                cmd.Start();
                cmd.WaitForExit();
            }

            Reboot();
        }

        private void Reboot()
        {
            DialogResult dialogResult = MessageBox.Show("Settings applied! Some changes may require a reboot to take effect. Woul you like to reboot now?", "PingOptimizer", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                ProcessStartInfo reboot = new("shutdown", "/r /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(reboot);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void RadioOptimal_CheckedChanged(object sender, EventArgs e)
        {
            Lock();
            Ecn.Text = "enabled";
            Rsc.Text = "disabled";
            Tuning.Text = "normal";
            Heuristics.Text = "disabled";
            Provider.Text = "ctcp";
            Rss.Text = "enabled";
            Maxsyn.Text = "2";
            Nonsack.Text = "disabled";
            Initialrto.Text = "2000";
            Timestamps.Text = "disabled";

            Tcpackfrequency.Text = "1";
            Tcpdelackticks.Text = "0";
            Tcpnodelay.Text = "1";
            Networkthrottlingindex.Text = "-1";
            Systemresponsiveness.Text = "0";
            Ttl.Text = "64";
            Maxuserport.Text = "65534";
            Tcptimedwaitdelay.Text = "30";
            Dnspriority.Text = "6";
            Hostpriority.Text = "5";
            Localpriority.Text = "4";
            Netbtpriority.Text = "7";
            Nonbesteffortlimit.Text = "0";
            Nla.Text = "1";
            Largesystemcache.Text = "0";
            Memoryallocationsize.Text = "3";
        }

        private void RadioDefault_CheckedChanged(object sender, EventArgs e)
        {
            Lock();
            Ecn.Text = "disabled";
            Rsc.Text = "disabled";
            Tuning.Text = "normal";
            Heuristics.Text = "disabled";
            Provider.Text = "cubic";
            Rss.Text = "enabled";
            Maxsyn.Text = "2";
            Nonsack.Text = "disabled";
            Initialrto.Text = "3000";
            Timestamps.Text = "";

            Tcpackfrequency.Text = "";
            Tcpdelackticks.Text = "";
            Tcpnodelay.Text = "";
            Networkthrottlingindex.Text = "10";
            Systemresponsiveness.Text = "20";
            Ttl.Text = "";
            Maxuserport.Text = "";
            Tcptimedwaitdelay.Text = "30";
            Dnspriority.Text = "2000";
            Hostpriority.Text = "500";
            Localpriority.Text = "499";
            Netbtpriority.Text = "2001";
            Nonbesteffortlimit.Text = "";
            Nla.Text = "";
            Largesystemcache.Text = "0";
            Memoryallocationsize.Text = "1";
        }

        private void RadioCustom_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control all in Controls)
            {
                all.Enabled = true;
            }
        }

        private void Ttl_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Ttl.Text, "[^0-9]"))
            {
                Ttl.Text = "";
            }
        }

        private void Dnspriority_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Dnspriority.Text, "[^0-9]"))
            {
                Dnspriority.Text = "";
            }
        }

        private void Hostpriority_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Hostpriority.Text, "[^0-9]"))
            {
                Hostpriority.Text = "";
            }
        }

        private void Localpriority_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Localpriority.Text, "[^0-9]"))
            {
                Localpriority.Text = "";
            }
        }

        private void Netbtpriority_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Netbtpriority.Text, "[^0-9]"))
            {
                Netbtpriority.Text = "";
            }
        }

        private void Maxuserport_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Maxuserport.Text, "[^0-9]"))
            {
                Maxuserport.Text = "";
            }
        }

        private void Tcptimedwaitdelay_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Tcptimedwaitdelay.Text, "[^0-9]"))
            {
                Tcptimedwaitdelay.Text = "";
            }
        }

        private void Nonbesteffortlimit_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Nonbesteffortlimit.Text, "[^0-9]"))
            {
                Nonbesteffortlimit.Text = "";
            }
        }

        private void Initialrto_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(Initialrto.Text, "[^0-9]"))
            {
                Initialrto.Text = "";
            }
        }
    }
}
