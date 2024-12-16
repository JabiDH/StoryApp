import { HttpClient, HttpParams, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, map, Observable, tap, throwError } from "rxjs";
import { PagedResult } from "../models/paged-result.model";
import { Story } from "../models/story.model";
import { storyApiUrls } from "../assets/configs/config"

@Injectable({
  providedIn: 'root'
})
export class StoriesService {
  constructor(private httpClient: HttpClient) { }

  getNewStories(pageNumber: number, pageSize: number) : Observable<PagedResult<Story>> {
    let params = new HttpParams()
      .set("pageNumber", pageNumber)
      .set("pageSize", pageSize);

    return this.httpClient.get<PagedResult<Story>>(storyApiUrls.newStories, { params: params })
    .pipe(
      tap(console.log),
      map(res => res),
      catchError(throwError)
    );
  }
}
