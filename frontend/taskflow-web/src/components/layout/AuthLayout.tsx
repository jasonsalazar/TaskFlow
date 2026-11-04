import type { ReactNode } from "react";

interface AuthLayoutProps {
  children: ReactNode;
}

export default function AuthLayout({ children }: AuthLayoutProps) {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 px-4 py-8">
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold tracking-tight">TaskFlow</h1>
          <p className="mt-2 text-sm text-muted-foreground">
            Plan, collaborate, and get things done.
          </p>
        </div>

        {children}
      </div>
    </div>
  );
}
