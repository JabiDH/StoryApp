import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { StoriesService } from './stories.service';
import { PagedResult } from '../models/paged-result.model';
import { Story } from '../models/story.model';
import { storyApiUrls } from '../assets/configs/config';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

describe('StoriesService', () => {
  let service: StoriesService;
  let httpTestingController: HttpTestingController;

  const mockResponse: PagedResult<Story> = {
    stories: [
      { id: 1, title: 'Story 1', url: 'http://story.com/1' },
      { id: 2, title: 'Story 2', url: 'http://story.com/2' },
    ] as Story[],
    totalRecords: 2,
    currentPage: 1,
    pageSize: 2,
    totalPages: 1
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
    imports: [],
    providers: [StoriesService, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});
    service = TestBed.inject(StoriesService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch new stories successfully', () => {
    const pageNumber = 1;
    const pageSize = 2;

    service.getNewStories(pageNumber, pageSize).subscribe((result) => {
      expect(result).toEqual(mockResponse);
      expect(result.stories.length).toBe(2);
      expect(result.totalRecords).toBe(2);
    });

    const req = httpTestingController.expectOne(
      `${storyApiUrls.getNewStories}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );

    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle HTTP error', () => {
    const pageNumber = 1;
    const pageSize = 2;
    const mockError = { status: 500, statusText: 'Internal Server Error' };

    service.getNewStories(pageNumber, pageSize).subscribe({
      next: () => fail('Expected an error, not stories'),
      error: (error) => {
        expect(error).toBeTruthy();
      }
    });

    const req = httpTestingController.expectOne(
      `${storyApiUrls.getNewStories}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );

    expect(req.request.method).toBe('GET');
    req.flush(null, mockError);
  });

  it('should send correct query parameters', () => {
    const pageNumber = 3;
    const pageSize = 5;

    service.getNewStories(pageNumber, pageSize).subscribe();

    const req = httpTestingController.expectOne(
      (req) => req.url === storyApiUrls.getNewStories
    );

    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('pageNumber')).toBe('3');
    expect(req.request.params.get('pageSize')).toBe('5');

    req.flush(mockResponse);
  });
});
