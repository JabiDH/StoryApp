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
import { catchError, Observable, of, Subscribable, Subscription, switchMap, throwError } from 'rxjs';

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
    AsyncPipe
  ],
  templateUrl: './stories.component.html',
  styleUrl: './stories.component.css'
})
export class StoriesComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['title', 'url'];
  storiesDataSource = new MatTableDataSource<any>();
  stories$: Observable<Story[]> = of();
  subscription!: Subscription;

  pageIndex: number = 0;
  pageSize: number = pagingConfigs.pageSize;
  pageSizeOptions:number[] = pagingConfigs.pageSizeOptions;
  totalRecords: number = 0;

  constructor(private storiesService: StoriesService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.stories$ = this.getNewStories(this.pageIndex, this.pageSize).pipe(
      switchMap((res: PagedResult<Story>) => {
        this.totalRecords = res.totalRecords;
        return [res.stories];
      }),
      catchError(this.handleError)
    );
    this.subscription = this.stories$.subscribe(stories => this.storiesDataSource.data = stories);
  }

  getNewStories(page: number, size: number) : Observable<PagedResult<Story>> {
    return this.storiesService.getNewStories(page + 1, size);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.storiesDataSource.filter = filterValue.trim().toLowerCase();
  }

  handleError(error: any): Observable<Story[]> {
    const errorMsg = 'An error occurred while fetching stories.';
    console.log(errorMsg)
    console.error(error);
    return of([]);
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
