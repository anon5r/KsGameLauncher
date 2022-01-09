using System.Security.Permissions;
using System.Windows.Forms;


namespace KsGameLauncher
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// Disabling "Close" button on control box
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int CP_NOCLOSE_BUTTON = 0x200;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return cp;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }
    }
}
