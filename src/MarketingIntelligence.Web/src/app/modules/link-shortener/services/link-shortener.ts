import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShortenedLink {
  shortCode: string;
  originalUrl: string;
  createdAt?: string;
}

export interface LinkStats {
  link: ShortenedLink;
  totalClicks: number;
  recentClicks: LinkClick[];
}

export interface LinkClick {
  ipAddress: string;
  clickedAt: string;
  userAgent: string;
}

@Injectable({
  providedIn: 'root'
})
export class LinkShortenerService {
  private apiUrl = '/api/links';

  constructor(private http: HttpClient) { }

  shortenUrl(originalUrl: string): Observable<ShortenedLink> {
    return this.http.post<ShortenedLink>(this.apiUrl, JSON.stringify(originalUrl), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  getStats(shortCode: string): Observable<LinkStats> {
    return this.http.get<LinkStats>(`${this.apiUrl}/${shortCode}/stats`);
  }
}
