import { useAuthStore } from "../../stores/authStore";
import { logout } from "../auth/api/logout";

export default function DashboardPage() {
  const user = useAuthStore((state) => state.user);

  return (
    <main className="min-h-screen p-8">
      <div className="mx-auto max-w-5xl">
        <h1 className="text-3xl font-bold">Dashboard</h1>

        <p className="mt-2 text-muted-foreground">Welcome, {user?.email}!</p>

        <button
          className="mt-6 rounded-md border px-4 py-2"
          onClick={() => void logout()}
        >
          Sign out
        </button>
      </div>
    </main>
  );
}
