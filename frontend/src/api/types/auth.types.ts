export interface LoginRequest {
  employeeNo: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  employeeNo: string;
  fullName: string;
  accessTokenExpiresAtUtc: string;
}

export interface AuthUser {
  employeeNo: string;
  fullName: string;
  isAdmin: boolean;
}