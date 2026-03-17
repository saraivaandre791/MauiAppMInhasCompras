using MauiAppMInhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMInhasCompras.Views;



public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> Lista = new ObservableCollection<Produto>() ;

	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = Lista;
	}

	protected async override void OnAppearing() 
	{
		List<Produto> tmp = await App.DB.GetAll();

		tmp.ForEach(i => Lista.Add(i));
		
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
		string q = e.NewTextValue;

		Lista.Clear();

        List<Produto> tmp = await App.DB.Search(q);

        tmp.ForEach(i => Lista.Add(i));
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		double soma = Lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecinado = sender as MenuItem;
            Produto p = selecinado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

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
}