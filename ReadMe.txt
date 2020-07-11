1. Create a folder in C Drive named as temp (This will be used to keep our file on local folder once we read it).
2. To test this applictaion, please run application from Visual Studio and then use api call from postmen. 
	We can bind this api call to any application.
3. Postmen call details:
	var client = new RestClient("https://localhost:44364/api/Reader/QRRead");
	client.Timeout = -1;
	var request = new RestRequest(Method.POST);
	request.AddHeader("Content-Type", "multipart/form-data");
	request.AddFile("file", "/C:/Users/aarora3/Pictures/Publish/QRCode/TransPerfact.png");
	IRestResponse response = client.Execute(request);
	Console.WriteLine(response.Content);