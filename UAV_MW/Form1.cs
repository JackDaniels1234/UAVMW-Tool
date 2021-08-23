using Squirrel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIBES
{
	public class Form1 : Form
	{
		private Memory m = new Memory();

		public VIBES.Threads Threads;

		public static bool GameAttached;

		public static Form1 main;

		private IContainer components;

		private Button UAV_ON;

		private BackgroundWorker backgroundWorker1;

		private Label label1;

		static Form1()
		{
			Form1.GameAttached = true;
			Form1.main = null;
		}

		public Form1()
		{
			this.InitializeComponent();
			checkForUpdates();
		}



		//Auto update method______________________________________________________________________________________________________________________________
		private async Task checkForUpdates()
		{


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			Task.Run(async () =>
			{
				try
				{
					using (var manager = UpdateManager.GitHubUpdateManager("https://github.com/JackDaniels1234/UAVTool").Result) //put github respo link here.
					{

						var re = await manager.UpdateApp();
						if (re != null)
						{
							MessageBox.Show("Updating to the latest version ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

							UpdateManager.RestartApp();

						}
					}
				}
				catch (Exception ex)
				{
					//MessageBox.Show(ex.Message);
				}
			});
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		//__________________________________________________________________________________________________




		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				try
				{
					if (Offsets.BaseAddress == 0)
					{
						Form1.GameAttached = false;
						for (int i = 0; i < 3; i++)
						{
							this.label1.Text = "WAITING FOR GAME";
							Thread.Sleep(500);
							this.label1.Text = "WAITING FOR GAME.";
							Thread.Sleep(500);
							this.label1.Text = "WAITING FOR GAME..";
							Thread.Sleep(500);
							this.label1.Text = "WAITING FOR GAME...";
							Thread.Sleep(500);
						}
					}
					else
					{
						Form1.GameAttached = true;
						this.label1.ForeColor = Color.Red;
						this.label1.Text = "READY!";
					}
				}
				catch
				{
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.start();
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
			this.UAV_ON = new Button();
			this.backgroundWorker1 = new BackgroundWorker();
			this.label1 = new Label();
			base.SuspendLayout();
			this.UAV_ON.BackColor = SystemColors.ActiveCaptionText;
			this.UAV_ON.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.UAV_ON.Location = new Point(11, 20);
			this.UAV_ON.Margin = new System.Windows.Forms.Padding(2);
			this.UAV_ON.Name = "Vibes.UAV_ON";
			this.UAV_ON.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.UAV_ON.Size = new System.Drawing.Size(266, 42);
			this.UAV_ON.TabIndex = 1;
			this.UAV_ON.Text = "ENABLE";
			this.UAV_ON.UseVisualStyleBackColor = true;
			this.UAV_ON.Click += new EventHandler(this.UAV_ON_Click);
			this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "WAITING FOR GAME";
			this.label1.Click += new EventHandler(this.label1_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = SystemColors.ActiveCaptionText;
			base.ClientSize = new System.Drawing.Size(288, 83);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.UAV_ON);
			this.ForeColor = Color.Red;
			this.BackColor = Color.Gray;
			base.Name = "Form1";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Text = "Warzone Hacker Hub";
			base.Load += new EventHandler(this.Form1_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void label1_Click(object sender, EventArgs e)
		{
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		public void start()
		{
			Form1.main = this;
			if (!this.backgroundWorker1.IsBusy)
			{
				this.backgroundWorker1.RunWorkerAsync();
			}
			this.Threads = new VIBES.Threads();
			(new Thread(new ThreadStart(this.Threads.PointerThread))
			{
				IsBackground = true
			}).Start();
		}

		private void UAV_ON_Click(object sender, EventArgs e)
		{
			if (Form1.GameAttached)
			{
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset1, new byte[] { 1, 0, 0, 0 });
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset2, new byte[] { 1, 0, 0, 0 });
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset3, new byte[] { 79, 187, 85, 81 });
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset4, new byte[] { 246, 109, 174, 121 });
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset5, new byte[] { 79, 187, 85, 81 });
				this.Threads.m.WriteBytes(Offsets.BaseAddress + Offsets.UAV_Offset6, new byte[] { 246, 109, 174, 121 });
			}
		}
	}
}