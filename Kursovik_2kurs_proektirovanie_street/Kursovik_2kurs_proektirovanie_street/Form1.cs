// Курсовик для летней сессии 
// Последнее обновление: 27.05.24 
// Добавлена: обработка клавиши iBtn_delete

// Что ещё нужно:   обработка клавиш: iBtn_nazad, iBtn_vpered;
//                  настроить повороты для картинок;
//                  сделать прозрачный фон для картинок;
//                  сделать так, чтобы одна картинка не налезала на другую;
//                  при выборе картинок не рисовала кисть
//                  отмасштабировать картинки(чтобы не изменяли размер

using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Ink;

namespace Kursovik_2kurs_proektirovanie_street
{
    public partial class Form1 : Form
    {
        // Пользовательская панель с двойной буферизацией
        private MyPanel main_panel; 

        // Переменные для работы с графикой в main_panel
        private bool peremeshenieNaMainPanel = false;
        private PictureBox vibrnElementPictureBox;
        private List<PictureBox> spisokRazmeshElements = new List<PictureBox>();

        // Переменные для рисования кистью
        private bool isDrawing = false; 
        private ForPointAndColor tekushShtrih;
        private List<ForPointAndColor> SpisokVsehShtrihs = new List<ForPointAndColor>();
        private Color tekushColor = Color.Black;
        public Form1()
        {
            InitializeComponent();
            InitializeMyPanel();
            InitializeColorButtons();
        }

        private void InitializeMyPanel()
        {
            // Инициализация MyPanel вместо обычного Panel
            main_panel = new MyPanel
            {
                Location = new Point(237, 58),
                Size = new Size(938, 583),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Gainsboro
            };
            this.Controls.Add(main_panel);

            // Поместить панель на передний план
            main_panel.BringToFront();

            // Настройка событий для main_panel
            main_panel.MouseDown += main_panel_MouseDown;
            main_panel.MouseMove += main_panel_MouseMove;
            main_panel.MouseUp += main_panel_MouseUp;
            main_panel.Paint += main_panel_Paint;
        } // Инициализация моей пользовательской панели
        private void InitializeColorButtons()
        {
            btn_red.Click += (sender, e) => tekushColor = Color.Red;
            btn_orange.Click += (sender, e) => tekushColor = Color.Orange;
            btn_yellow.Click += (sender, e) => tekushColor = Color.Yellow;
            btn_green.Click += (sender, e) => tekushColor = Color.Green;
            btn_lightblue.Click += (sender, e) => tekushColor = Color.LightBlue;
            btn_blue.Click += (sender, e) => tekushColor = Color.Blue;
            btn_purple.Click += (sender, e) => tekushColor = Color.Purple;
            btn_black.Click += (sender, e) => tekushColor = Color.Black;
            btn_white.Click += (sender, e) => tekushColor = Color.White;
        } // Инициализация палитры для кисточки
        public class ForPointAndColor
        {
            public List<Point> Points { get; set; }
            public Color Color { get; set; }

            public ForPointAndColor(Color color)
            {
                Points = new List<Point>();
                Color = color;
            }
        } // Класс, который будет хранить точки и цвет каждого штриха.

        // Переменные для перетаскивания формы
        bool peretaskivanie = false;
        Point polozhenieCursoraNow;
        Point polozhenieFormNow;

        // Цвет активной кнопки
        private Color activeBackgroundColor = Color.FromArgb(52, 52, 52);
        private Color activeForegroundColor = Color.FromArgb(237, 178, 28);

        // Цвет кнопки изначально 
        private Color iznachalnoBackgroundColor = Color.FromArgb(46, 46, 50);
        private Color iznachalnoForegroundColor = Color.FromArgb(255, 255, 255);


        // Обработка события кнопок //
        private void iBtn_paint_Click(object sender, EventArgs e)
        {
            isDrawing = !isDrawing; // Переключение режима рисования
            main_panel.Cursor = isDrawing ? Cursors.Cross : Cursors.Default;
            if (isDrawing)
            {
                pnl_for_painting.Visible = true;
            }
            else
            {
                pnl_for_painting.Visible= false;
            }
        }  // Кнопка для рисования кистью на панели
        private void iBtn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        } // Кнопка выхода в вернем правом углу окна
        private void iBtn_vihod_Click(object sender, EventArgs e)
        {
            Close();
        } // Кнопка выхода в нижнем левом углу окна
        private void iBtn_minimum_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        } // Кнопка сворачивания окна
        private void iBtn_zdaniya_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_zdanie = (IconButton)sender;
            SetButtonColors(iBtn_zdanie, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel1.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "здание"
            DlyaClickButtons(sender, "здание.png");
        } // Кнопка для добавления здания
        private void iBtn_rastenie_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_rastenie = (IconButton)sender;
            SetButtonColors(iBtn_rastenie, activeBackgroundColor, activeForegroundColor);

            l_panel2.Visible = true;

            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);

            l_panel1.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "растение"
            DlyaClickButtons(sender, "растение.png");
        } // Кнопка для добавления растения
        private void iBtn_doroga_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_doroga = (IconButton)sender;
            SetButtonColors(iBtn_doroga, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel3.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "дорога"
            DlyaClickButtons(sender, "дорога.png");
        } // Кнопка для добавления автомобильной дороги
        private void iBtn_parkovka_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_parkovka = (IconButton)sender;
            SetButtonColors(iBtn_parkovka, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel4.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "парковка"
            DlyaClickButtons(sender, "парковка.png");
        } // Кнопка для добавления парковочных мест
        private void iBtn_transport_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_transport = (IconButton)sender;
            SetButtonColors(iBtn_transport, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel5.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "транспорт"
            DlyaClickButtons(sender, "транспорт.png");
        } // Кнопка для добавления транспортных средств
        private void iBtn_osveshenie_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_osveshenie = (IconButton)sender;
            SetButtonColors(iBtn_osveshenie, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel6.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel7.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "освещение"
            DlyaClickButtons(sender, "освещение.png");
        } // Кнопка для добавления уличных фонарей
        private void iBtn_chelovek_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_chelovek = (IconButton)sender;
            SetButtonColors(iBtn_chelovek, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel7.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_trotuar, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel8.Visible = false;

            // Выбор элемента "человек"
            DlyaClickButtons(sender, "человек.png");
        } // Кнопка для добавления людей
        private void iBtn_trotuar_Click(object sender, EventArgs e)
        {
            // Изменение цвета при нажатии
            IconButton iBtn_trotuar = (IconButton)sender;
            SetButtonColors(iBtn_trotuar, activeBackgroundColor, activeForegroundColor);
            // Видимость активной панели
            l_panel8.Visible = true;
            // Все остальные кнопки возвращаются к изначальному значению цвета
            SetButtonColors(iBtn_rastenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_zdanie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_doroga, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_parkovka, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_transport, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_osveshenie, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            SetButtonColors(iBtn_chelovek, iznachalnoBackgroundColor, iznachalnoForegroundColor);
            // Все остальные панели возвращаются к изначальному значению цвета
            l_panel1.Visible = false;
            l_panel2.Visible = false;
            l_panel3.Visible = false;
            l_panel4.Visible = false;
            l_panel5.Visible = false;
            l_panel6.Visible = false;
            l_panel7.Visible = false;

            // Выбор элемента "тротуар"
            DlyaClickButtons(sender, "тротуар.png");
        } // Кнопка для добавления тротуаров
        private void iBtn_nazad_Click(object sender, EventArgs e)
        {

        } // Кнопка для отмены(назад)
        private void iBtn_vpered_Click(object sender, EventArgs e)
        {

        } // Кнопка для возвращения(вперед)
        private void iBtn_delete_Click(object sender, EventArgs e)
        {
            // Очистка MyPanel
            main_panel.Controls.Clear();
            // Очистка списка штрихов
            SpisokVsehShtrihs.Clear();
            // Очистка списка добавленных изображений
            spisokRazmeshElements.Clear();
            // Обновление отображения
            main_panel.Invalidate();
        } // Кнопка для очистки всего MyPanel
        private void iBtn_save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JPEG Image|*.jpg";
                saveFileDialog.FileName = "street.jpg";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SavePanelAsImage(saveFileDialog.FileName);
                }
            }
        } // Кнопка для сохранения картинки в формате JPEG
        private void SetButtonColors(IconButton button, Color backColor, Color foreColor)
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.IconColor = foreColor;
        } // Метод для изменения цветов
        private void SavePanelAsImage(string fileName)
        {
            Bitmap bitmap = new Bitmap(main_panel.Width, main_panel.Height);
            main_panel.DrawToBitmap(bitmap, new Rectangle(0, 0, main_panel.Width, main_panel.Height));
            bitmap.Save(fileName, ImageFormat.Jpeg);
        } // Метод для сохранения панели как картинки(jpeg)
        private void DlyaClickButtons(object sender, string elementName)
        {
            // Изменение цвета при нажатии
            IconButton clickedButton = (IconButton)sender;
            SetButtonColors(clickedButton, activeBackgroundColor, activeForegroundColor);

            // Сброс цветов остальных кнопок и скрытие панелей
            foreach (Control control in main_panel.Controls)
            {
                if (control is IconButton button && button != clickedButton)
                {
                    SetButtonColors(button, iznachalnoBackgroundColor, iznachalnoForegroundColor);
                }
            }

            // Создание нового PictureBox для выбранного элемента
            vibrnElementPictureBox = SozdanieElementaPictureBox(elementName);
        } // Метод для обработки нажатий кнопок, изменением их цветов и созданием нового PictureBox
        private PictureBox SozdanieElementaPictureBox(string fileName)
        {
            main_panel.Cursor = Cursors.Cross;
            peremeshenieNaMainPanel = true;
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures", fileName);

            PictureBox pictureBox = new PictureBox
            {
                Image = new Bitmap(imagePath),
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            pictureBox.MouseClick += PictureBox_MouseClick;

            return pictureBox;
        } // Метод для создания PictureBox с изображением из файла


        // События панелей //
        private void main_panel_MouseDown(object sender, MouseEventArgs e)
        {
            // Если включен режим рисования и нажата левая кнопка мыши
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                tekushShtrih = new ForPointAndColor(tekushColor); // Создаем новый штрих с текущим цветом
                tekushShtrih.Points.Add(e.Location); // Добавляем точку начала штриха
            }
            // Если активирован режим перемещения и выбран элемент PictureBox
            else if (peremeshenieNaMainPanel && vibrnElementPictureBox != null)
            {
                vibrnElementPictureBox.Location = e.Location; // Устанавливаем расположение элемента PictureBox
            }
            // Если активирован режим перемещения и выбран элемент PictureBox
            if (peremeshenieNaMainPanel && vibrnElementPictureBox != null)
            {
                vibrnElementPictureBox.Location = e.Location; // Устанавливаем расположение элемента PictureBox
                peremeshenieNaMainPanel = false; // Сбрасываем флаг перемещения

                main_panel.Controls.Add(vibrnElementPictureBox); // Добавляем элемент PictureBox на main_panel
                spisokRazmeshElements.Add(vibrnElementPictureBox); // Добавляем элемент PictureBox в список размещенных элементов
                vibrnElementPictureBox = null; // Сбрасываем выбранный элемент PictureBox

                main_panel.Invalidate(); // Обновляем отображение main_panel
            }
            // Если нажата правая кнопка мыши и выбран элемент PictureBox
            else if (e.Button == MouseButtons.Right && vibrnElementPictureBox != null)
            {
                contextMenuStrip1.Show(main_panel, e.Location); // Показываем контекстное меню
            }
        } // Обработчик события MouseDown для main_panel
        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PictureBox pictureBox = sender as PictureBox; // Получаем элемент PictureBox
                if (pictureBox != null)
                {
                    contextMenuStrip1.Tag = pictureBox; // Сохраняем PictureBox в Tag контекстного меню
                    contextMenuStrip1.Show(pictureBox, e.Location); // Показываем контекстное меню у PictureBox
                }
            }
        } // Обработчик события MouseClick для PictureBox
        private void main_panel_MouseMove(object sender, MouseEventArgs e)
        {
            // Если включен режим рисования и нажата левая кнопка мыши
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                tekushShtrih.Points.Add(e.Location); // Добавляем точку к текущему штриху
                main_panel.Invalidate(); // Обновляем отображение main_panel
            }
            // Если активирован режим перемещения и выбран элемент PictureBox
            else if (peremeshenieNaMainPanel && vibrnElementPictureBox != null)
            {
                vibrnElementPictureBox.Location = e.Location; // Устанавливаем расположение элемента PictureBox
                main_panel.Invalidate(); // Обновляем отображение main_panel
            }
        } // Обработчик события MouseMove для main_panel  
        private void main_panel_MouseUp(object sender, MouseEventArgs e)
        {
            // Если включен режим рисования и нажата левая кнопка мыши
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                SpisokVsehShtrihs.Add(tekushShtrih); // Добавляем текущий штрих в список всех штрихов
                tekushShtrih = null; // Сбрасываем текущий штрих
                main_panel.Invalidate(); // Обновляем отображение main_panel
            }
            // Если активирован режим перемещения и выбран элемент PictureBox
            else if (peremeshenieNaMainPanel && vibrnElementPictureBox != null)
            {
                vibrnElementPictureBox.Location = e.Location; // Устанавливаем расположение элемента PictureBox
                peremeshenieNaMainPanel = false; // Сбрасываем флаг перемещения

                main_panel.Controls.Add(vibrnElementPictureBox); // Добавляем элемент PictureBox на main_panel
                spisokRazmeshElements.Add(vibrnElementPictureBox); // Добавляем элемент PictureBox в список размещенных элементов
                vibrnElementPictureBox = null; // Сбрасываем выбранный элемент PictureBox

                main_panel.Invalidate(); // Обновляем отображение main_panel
            }
        } // Обработчик события MouseUp для main_panel
        private void main_panel_Paint(object sender, PaintEventArgs e) 
        {
            // Для рисования всех предыдущих штрихов
            foreach (var stroke in SpisokVsehShtrihs)
            {
                if (stroke.Points.Count > 1)
                {
                    using (Pen pen = new Pen(stroke.Color, 2))
                    {
                        for (int i = 1; i < stroke.Points.Count; i++)
                        {
                            e.Graphics.DrawLine(pen, stroke.Points[i - 1], stroke.Points[i]); // Рисуем линию между точками
                        }
                    }
                }
            }

            // Для рисования текущего штриха
            if (tekushShtrih != null && tekushShtrih.Points.Count > 1)
            {
                using (Pen pen = new Pen(tekushShtrih.Color, 2))
                {
                    for (int i = 1; i < tekushShtrih.Points.Count; i++)
                    {
                        e.Graphics.DrawLine(pen, tekushShtrih.Points[i - 1], tekushShtrih.Points[i]); // Рисуем линию между точками
                    }
                }
            }

            // Если активирован режим перемещения и выбран элемент PictureBox
            if (peremeshenieNaMainPanel && vibrnElementPictureBox != null)
            {
                e.Graphics.DrawImage(vibrnElementPictureBox.Image, vibrnElementPictureBox.Location); // Рисуем изображение
            }
        } // Обработчик события Paint для main_panel
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            peretaskivanie = true; // Устанавливаем флаг перемещения в true
            polozhenieCursoraNow = Cursor.Position; // Сохраняем текущую позицию курсора
            polozhenieFormNow = this.Location; // Сохраняем текущую позицию формы
        }  // Обработчик события MouseDown для panel1 (для перемещения формы)
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            peretaskivanie = false; // Сбрасываем флаг перемещения
        } // Обработчик события MouseUp для panel1 (для перемещения формы)
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (peretaskivanie)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(polozhenieCursoraNow)); // Вычисляем разницу в позиции курсора
                this.Location = Point.Add(polozhenieFormNow, new Size(dif)); // Устанавливаем новую позицию формы
            }
        } // Обработчик события MouseMove для panel1 (для перемещения формы)


        // Обработка событий контекстного меню //
        private void повернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag is PictureBox pictureBox)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Invalidate();
            }
        } // Обработка события поворота на 90 градусов
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag is PictureBox pictureBox)
            {
                main_panel.Controls.Remove(pictureBox);
                spisokRazmeshElements.Remove(pictureBox);
                pictureBox.Dispose();
            }
        } // Обработка события удаления элемента
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == удалитьToolStripMenuItem)
            {
                if (contextMenuStrip1.Tag is PictureBox pictureBox)
                {
                    UdaleniePictureBox(pictureBox);
                }
            }
            else if (e.ClickedItem == повернутьToolStripMenuItem)
            {
                PovorotPictureBox(90); // Поворот на 90 градусов
            }
        } // Обработка события для удаления или поворота элементов
        private void PovorotPictureBox(float angle)
        {
            if (contextMenuStrip1.Tag is PictureBox pictureBox)
            {
                // Получение текущего изображения
                Image img = pictureBox.Image;

                // Создание нового Bitmap с учетом нового размера после поворота
                Bitmap rotatedImage = new Bitmap(img.Height, img.Width);
                using (Graphics g = Graphics.FromImage(rotatedImage))
                {
                    // Установка точки вращения в центр изображения
                    g.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2);
                    // Выполнение поворота
                    g.RotateTransform(angle);
                    // Восстановление координат и отрисовка изображения
                    g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                    g.DrawImage(img, new Point(0, 0));
                }

                // Установка нового изображения и обновление PictureBox
                pictureBox.Image = rotatedImage;
                pictureBox.Size = rotatedImage.Size;
                pictureBox.Invalidate();
            }
        } // Метод для поворота картинки
        private void UdaleniePictureBox(PictureBox pictureBox)
        {
            main_panel.Controls.Remove(pictureBox);
            spisokRazmeshElements.Remove(pictureBox);
            pictureBox.Dispose();
        } // Метод для удаления картинки
    }
}
