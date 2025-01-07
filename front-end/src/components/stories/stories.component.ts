import { Component, OnDestroy, OnInit } from '@angular/core';
import { StoriesService } from '../../services/stories.service';
import { PagedResult } from '../../models/paged-result.model';
import { Story } from '../../models/story.model';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { pagingConfigs } from '../../assets/configs/config';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AsyncPipe, CommonModule, JsonPipe } from '@angular/common';
import { BehaviorSubject, catchError, Observable, of, Subscribable, Subscription, switchMap, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    JsonPipe,
    AsyncPipe,
    FormsModule
  ],
  templateUrl: './stories.component.html',
  styleUrl: './stories.component.css'
})
export class StoriesComponent implements OnInit {
  title: string = 'Stories';
  displayedColumns: string[] = ['title', 'url'];
  searchValue: string = '';

  pageIndex: number = 0;
  pageSize: number = pagingConfigs.pageSize;
  pageSizeOptions: number[] = pagingConfigs.pageSizeOptions;
  totalRecords: number = 0;

  storiesSub = new BehaviorSubject({ pageIndex: this.pageIndex, pageSize: this.pageSize });
  stories$!: Observable<any>;

  constructor(private storiesService: StoriesService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.stories$ = this.storiesSub.pipe(
      switchMap((pageData) => {
        const getStories = this.searchValue ? this.searchNewStories(pageData.pageIndex, pageData.pageSize, this.searchValue) :
          this.getNewStories(this.pageIndex, this.pageSize);

        return getStories.pipe(
          switchMap((res: PagedResult<Story>) => {
            this.totalRecords = res.totalRecords;
            return [res.stories];
          })
        );
      })
    );
  }

  searchData() {
    this.pageIndex = 0;
    this.loadData();
  }

  getNewStories(page: number, size: number): Observable<PagedResult<Story>> {
    return this.storiesService.getNewStories(page + 1, size);
  }

  searchNewStories(page: number, size: number, search: string): Observable<PagedResult<Story>> {
    return this.storiesService.searchNewStories(page + 1, size, search);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.storiesSub.next({pageIndex: this.pageIndex, pageSize: this.pageSize});
  }

  handleError(error: any): Observable<Story[]> {
    const errorMsg = 'An error occurred while fetching stories.';
    console.log(errorMsg)
    console.error(error);
    return of([]);
  }
}
