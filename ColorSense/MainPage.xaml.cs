using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ColorSense
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public string CorNomes(int red, int green, int blue)
        {
            // Lista de cores básicas com seus nomes e valores RGB
            var colors = new Dictionary<string, (int Red, int Green, int Blue)>()
    {
{ "Vermelho", (255, 0, 0) },
{ "Vermelho Escuro", (139, 0, 0) },
{ "Laranja Vermelha", (255, 69, 0) },

{ "Laranja Escura", (255, 140, 0) },
{ "Laranja", (255, 165, 0) },
{ "Dourado", (255, 215, 0) },

{ "Amarelo", (255, 255, 0) },
{ "Amarelo Claro", (255, 255, 224) },

{ "Verde", (0, 255, 0) },
{ "Verde Floresta", (34, 139, 34) },
{ "Verde Escuro", (0, 128, 0) },
{ "Verde Claro", (144, 238, 144) },

{ "Azul", (0, 0, 255) },
{ "Azul Meia-noite", (25, 25, 112) },
{ "Azul Aço", (70, 130, 180) },
{ "Azul Claro", (173, 216, 230) },

{ "Ciano", (0, 255, 255) },
{ "Ciano Claro", (224, 255, 255) },

{ "Magenta", (255, 0, 255) },
{ "Rosa Forte", (255, 20, 147) },
{ "Rosa Claro", (255, 192, 203) },

{ "Roxo", (128, 0, 128) },
{ "Roxo Escuro", (148, 0, 211) },

{ "Marrom", (165, 42, 42) },

{ "Cinza", (128, 128, 128) },
{ "Cinza Escuro", (169, 169, 169) },
{ "Cinza Claro", (211, 211, 211) },

{ "Preto", (0, 0, 0) },
{ "Branco", (255, 255, 255) },
{ "Branco Fumaça", (245, 245, 245) },

{ "Verde Oliva", (107, 142, 35) },
{ "Verde Escuro Floresta", (0, 100, 0) }
    };

            string closestColor = null;
            double minDistance = double.MaxValue;

            foreach (var color in colors)
            {
                // Cálculo da distância euclidiana
                double distance = Math.Sqrt(
                    Math.Pow(red - color.Value.Red, 2) +
                    Math.Pow(green - color.Value.Green, 2) +
                    Math.Pow(blue - color.Value.Blue, 2)
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = color.Key;
                }
            }

            return closestColor;
        }


        private async void SelecionarFoto(object sender, EventArgs e)
        {
            try
            {
                // Seleciona uma foto da galeria
                var photo = await MediaPicker.PickPhotoAsync();

                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    {
                        // Ler os bytes da imagem para análise
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();

                        // Criar ImageSource a partir do stream
                        var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        Imagem.Source = imageSource;

                        Imagem.IsVisible = true;
                        CorDominante.IsVisible = true;
                        ColorLabel.IsVisible = true;

                        // Identificar cor predominante
                        var dominantColor = PegarCorDominante(imageBytes);

                        // Exibir a cor predominante
                        CorDominante.Color = Xamarin.Forms.Color.FromRgb(dominantColor.Red, dominantColor.Green, dominantColor.Blue);

                        // Exibir o nome da cor
                        var colorName = CorNomes(dominantColor.Red, dominantColor.Green, dominantColor.Blue);
                        ColorLabel.Text = $"Cor predominante: {colorName}";

                        // Fala o nome da cor
                        await TextToSpeech.SpeakAsync(colorName);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível selecionar a foto: {ex.Message}", "OK");
            }
        }

        private async void CapturarFoto(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    {
                        // Ler os bytes da imagem para análise
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();

                        // Criar ImageSource a partir do stream
                        var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        Imagem.Source = imageSource;

                        Imagem.IsVisible = true;
                        CorDominante.IsVisible = true;
                        ColorLabel.IsVisible = true;

                        // Identificar cor predominante
                        var dominantColor = PegarCorDominante(imageBytes);

                        // Exibir a cor predominante
                        CorDominante.Color = Xamarin.Forms.Color.FromRgb(dominantColor.Red, dominantColor.Green, dominantColor.Blue);

                        // Exibir o nome da cor
                        var colorName = CorNomes(dominantColor.Red, dominantColor.Green, dominantColor.Blue);
                        ColorLabel.Text = $"Cor predominante: {colorName}";

                        // Fala o nome da cor
                        await TextToSpeech.SpeakAsync(colorName);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível capturar a foto: {ex.Message}", "OK");
            }
        }

        private SKColor PegarCorDominante(byte[] imageData)
        {
            using (var bitmap = SKBitmap.Decode(imageData))
            {
                // Definir uma área central (25% do tamanho da imagem)
                int startX = bitmap.Width / 4;
                int startY = bitmap.Height / 4;
                int width = bitmap.Width / 2;
                int height = bitmap.Height / 2;

                // Variáveis para armazenar a soma dos valores RGB
                long totalRed = 0, totalGreen = 0, totalBlue = 0;
                int pixelCount = 0;

                // Somar os valores RGB de cada pixel na área central
                for (int x = startX; x < startX + width; x++)
                {
                    for (int y = startY; y < startY + height; y++)
                    {
                        var color = bitmap.GetPixel(x, y);

                        totalRed += color.Red;
                        totalGreen += color.Green;
                        totalBlue += color.Blue;
                        pixelCount++;
                    }
                }

                // Calcular a média dos valores RGB
                int avgRed = (int)(totalRed / pixelCount);
                int avgGreen = (int)(totalGreen / pixelCount);
                int avgBlue = (int)(totalBlue / pixelCount);

                // Retornar a cor média
                return new SKColor((byte)avgRed, (byte)avgGreen, (byte)avgBlue);
            }
        }


    }
}
