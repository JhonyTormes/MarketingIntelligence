import { TestBed } from '@angular/core/testing';

import { LinkShortener } from './link-shortener';

describe('LinkShortener', () => {
  let service: LinkShortener;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LinkShortener);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
