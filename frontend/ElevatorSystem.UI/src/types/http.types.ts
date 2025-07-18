export interface ErrorResponseData {
  message?: string;
  [key: string]: unknown;
}

export interface HttpErrorResponse {
  response?: {
    data?: ErrorResponseData;
    status: number;
  };
  request?: unknown;
  message?: string;
}
