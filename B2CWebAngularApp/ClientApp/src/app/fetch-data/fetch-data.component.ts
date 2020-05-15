import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent implements OnInit {

  public forecasts: WeatherForecast[];

  constructor(private readonly http: HttpClient) {

  }

  ngOnInit(): void {
    const apiUrl = "https://localhost:44379/weatherforecast";

    this.http.get<WeatherForecast[]>(apiUrl).subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
