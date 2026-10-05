export type FieldType = 'text' | 'number' | 'textarea' | 'select';

export type FormField = {
  key: string;
  label: string;
  type: FieldType;
  required: boolean;
  options?: string[];
};

export type FormSchema = {
  fields: FormField[];
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  roles: string[];
  userId: string;
};

export type RequestStatus = 0 | 1 | 2;

export type RequestItem = {
  id: string;
  title: string;
  amount: number;
  description?: string;
  urgency: string;
  status: RequestStatus;
  createdByUserId: string;
  assignedRole: string;
  createdAt: string;
  decisionAt?: string;
};