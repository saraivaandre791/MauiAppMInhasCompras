using MauiAppMInhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMInhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        ObservableCollection<Produto> Lista = new ObservableCollection<Produto>();

        public ListaProduto()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = Lista;
        }

        protected async override void OnAppearing()
        {
            try
            {
                Lista.Clear();
                var tmp = await App.DB.GetAll();
                tmp.ForEach(i => Lista.Add(i));

                // Preencher categorias no Picker
                var categorias = tmp.Select(p => p.Categoria).Distinct().ToList();
                picker_categoria.ItemsSource = categorias;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NovoProduto());
        }

        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string q = e.NewTextValue;
                lst_produtos.IsRefreshing = true;
                Lista.Clear();
                var tmp = await App.DB.Search(q);
                tmp.ForEach(i => Lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            double soma = Lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            DisplayAlert("Total dos Produtos", msg, "OK");
        }

        // Relatório por categoria
        private async void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
        {
            try
            {
                var produtos = await App.DB.GetAll();

                var relatorio = produtos
                    .GroupBy(p => p.Categoria)
                    .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) })
                    .ToList();

                string msg = string.Join("\n", relatorio.Select(r => $"{r.Categoria}: {r.Total:C}"));

                await DisplayAlert("Relatório de Gastos por Categoria", msg, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecinado = sender as MenuItem;
                Produto p = selecinado.BindingContext as Produto;

                bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
                if (confirm)
                {
                    await App.DB.Delete(p.Id);
                    Lista.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Produto p = e.SelectedItem as Produto;
                if (p != null)
                {
                    Navigation.PushAsync(new EditarProduto
                    {
                        BindingContext = p,
                    });
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                Lista.Clear();
                var tmp = await App.DB.GetAll();
                tmp.ForEach(i => Lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }

        private async void picker_categoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (picker_categoria.SelectedItem == null)
                    return;

                string categoriaSelecionada = picker_categoria.SelectedItem.ToString();

                Lista.Clear();
                var produtos = await App.DB.SearchByCategoria(categoriaSelecionada);
                produtos.ForEach(i => Lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_produtos.IsRefreshing = false;
            }
        }
    }
}
